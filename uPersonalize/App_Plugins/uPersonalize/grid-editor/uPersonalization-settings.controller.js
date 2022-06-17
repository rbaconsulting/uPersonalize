angular.module("umbraco").controller("uPersonalize.PropertyEditors.GridController", function (
	$scope,
	localizationService,
	gridService,
	umbRequestHelper,
	angularHelper,
	$element,
	eventsService,
	editorService,
	overlayService,
	$interpolate) {

	// Grid status variables
	var placeHolder = "";
	var currentForm = angularHelper.getCurrentForm($scope);

	$scope.currentRowWithActiveChild = null;
	$scope.currentCellWithActiveChild = null;
	$scope.active = null;

	$scope.currentRow = null;
	$scope.currentCell = null;
	$scope.currentToolsControl = null;
	$scope.currentControl = null;

	$scope.openRTEToolbarId = null;
	$scope.hasSettings = false;
	$scope.showRowConfigurations = true;
	$scope.sortMode = false;
	$scope.reorderKey = "general_reorder";

	var shouldApply = function (item, itemType, gridItem) {
		if (item.applyTo === undefined || item.applyTo === null || item.applyTo === "") {
			return true;
		}

		if (typeof (item.applyTo) === "string") {
			return item.applyTo === itemType;
		}

		if (itemType === "row") {
			if (item.applyTo.row === undefined) {
				return false;
			}
			if (item.applyTo.row === null || item.applyTo.row === "") {
				return true;
			}
			var rows = item.applyTo.row.split(',');
			return _.indexOf(rows, gridItem.name) !== -1;
		} else if (itemType === "cell") {
			if (item.applyTo.cell === undefined) {
				return false;
			}
			if (item.applyTo.cell === null || item.applyTo.cell === "") {
				return true;
			}
			var cells = item.applyTo.cell.split(',');
			var cellSize = gridItem.grid.toString();
			return _.indexOf(cells, cellSize) !== -1;
		}
	}

	$scope.editGridItemSettings = function (gridItem, itemType) {

		placeHolder = "{0}";

		var styles, config;
		if (itemType === 'control') {
			styles = null;
			config = Utilities.copy(gridItem.editor.config.settings);
		} else {
			styles = _.filter(Utilities.copy($scope.model.config.items.styles), function (item) { return shouldApply(item, itemType, gridItem); });
			config = _.filter(Utilities.copy($scope.model.config.items.config), function (item) { return shouldApply(item, itemType, gridItem); });
		}

		if (Utilities.isObject(gridItem.config)) {
			_.each(config, function (cfg) {
				var val = gridItem.config[cfg.key];
				if (val) {
					cfg.value = stripModifier(val, cfg.modifier);
				}
			});
		}

		if (Utilities.isObject(gridItem.styles)) {
			_.each(styles, function (style) {
				var val = gridItem.styles[style.key];
				if (val) {
					style.value = stripModifier(val, style.modifier);
				}
			});
		}

		var dialogOptions = {
			view: "views/propertyeditors/grid/dialogs/config.html",
			size: "small",
			styles: styles,
			config: config,
			submit: function (model) {
				var styleObject = {};
				var configObject = {};

				_.each(model.styles, function (style) {
					if (style.value) {
						styleObject[style.key] = addModifier(style.value, style.modifier);
					}
				});
				_.each(model.config, function (cfg) {
					cfg.alias = cfg.key;
					cfg.label = cfg.value;

					if (cfg.value) {
						configObject[cfg.key] = addModifier(cfg.value, cfg.modifier);
					}
				});

				gridItem.styles = styleObject;
				gridItem.config = configObject;
				gridItem.hasConfig = gridItemHasConfig(styleObject, configObject);

				currentForm.$setDirty();

				editorService.close();
			},
			close: function () {
				editorService.close();
			}
		};

		localizationService.localize("general_settings").then(value => {
			dialogOptions.title = value;
			editorService.open(dialogOptions);
		});

	};

	function stripModifier(val, modifier) {
		if (!val || !modifier || modifier.indexOf(placeHolder) < 0) {
			return val;
		} else {
			var paddArray = modifier.split(placeHolder);
			if (paddArray.length == 1) {
				if (modifier.indexOf(placeHolder) === 0) {
					return val.slice(0, -paddArray[0].length);
				} else {
					return val.slice(paddArray[0].length, 0);
				}
			} else {
				if (paddArray[1].length === 0) {
					return val.slice(paddArray[0].length);
				}
				return val.slice(paddArray[0].length, -paddArray[1].length);
			}
		}
	}

	var addModifier = function (val, modifier) {
		if (!modifier || modifier.indexOf(placeHolder) < 0) {
			return val;
		} else {
			return modifier.replace(placeHolder, val);
		}
	};

	function gridItemHasConfig(styles, config) {

		if (_.isEmpty(styles) && _.isEmpty(config)) {
			return false;
		} else {
			return true;
		}

	}


	// *********************************************
	// Control management functions
	// *********************************************


	$scope.getTemplateName = function (control) {
		var templateName = control.editor.name;
		if (control.editor.nameExp) {
			var valueOfTemplate = control.editor.nameExp(control);
			if (valueOfTemplate != "") {
				templateName += ": ";
				templateName += valueOfTemplate;
			}
		}
		return templateName;
	}

	// *********************************************
	// Initialization
	// these methods are called from ng-init on the template
	// so we can controll their first load data
	//
	// intialization sets non-saved data like percentage sizing, allowed editors and
	// other data that should all be pre-fixed with $ to strip it out on save
	// *********************************************

	// *********************************************
	// Init template + sections
	// *********************************************
	$scope.initContent = function () {
		var clear = true;

		//settings indicator shortcut
		if (($scope.model.config.items.config && $scope.model.config.items.config.length > 0) || ($scope.model.config.items.styles && $scope.model.config.items.styles.length > 0)) {
			$scope.hasSettings = true;
		}

		//ensure the grid has a column value set,
		//if nothing is found, set it to 12
		if (!$scope.model.config.items.columns) {
			$scope.model.config.items.columns = 12;
		} else if (Utilities.isString($scope.model.config.items.columns)) {
			$scope.model.config.items.columns = parseInt($scope.model.config.items.columns);
		}

		if ($scope.model.value && $scope.model.value.sections && $scope.model.value.sections.length > 0 && $scope.model.value.sections[0].rows && $scope.model.value.sections[0].rows.length > 0) {

			if ($scope.model.value.name && Utilities.isArray($scope.model.config.items.templates)) {

				//This will occur if it is an existing value, in which case
				// we need to determine which layout was applied by looking up
				// the name
				// TODO: We need to change this to an immutable ID!!

				var found = _.find($scope.model.config.items.templates, function (t) {
					return t.name === $scope.model.value.name;
				});

				if (found && Utilities.isArray(found.sections) && found.sections.length === $scope.model.value.sections.length) {

					//Cool, we've found the template associated with our current value with matching sections counts, now we need to
					// merge this template data on to our current value (as if it was new) so that we can preserve what is and isn't
					// allowed for this template based on the current config.

					_.each(found.sections, function (templateSection, index) {
						Utilities.extend($scope.model.value.sections[index], Utilities.copy(templateSection));
					});

				}
			}

			_.forEach($scope.model.value.sections, function (section, index) {

				if (section.grid > 0) {
					$scope.initSection(section);

					//we do this to ensure that the grid can be reset by deleting the last row
					if (section.rows.length > 0) {
						clear = false;
					}
				} else {
					$scope.model.value.sections.splice(index, 1);
				}
			});
		} else if ($scope.model.config.items.templates && $scope.model.config.items.templates.length === 1) {
			$scope.addTemplate($scope.model.config.items.templates[0]);
			clear = false;
		}

		if (clear) {
			$scope.model.value = undefined;
		}
	};

	// *********************************************
	// Init layout / row
	// *********************************************
	$scope.initRow = function (row) {

		//merge the layout data with the original config data
		//if there are no config info on this, splice it out
		var original = _.find($scope.model.config.items.layouts, function (o) { return o.name === row.name; });

		if (!original) {
			return null;
		} else {
			//make a copy to not touch the original config
			original = Utilities.copy(original);
			original.styles = row.styles;
			original.config = row.config;
			original.hasConfig = gridItemHasConfig(row.styles, row.config);


			//sync area configuration
			_.each(original.areas, function (area, areaIndex) {


				if (area.grid > 0) {
					var currentArea = row.areas[areaIndex];

					if (currentArea) {
						area.config = currentArea.config;
						area.styles = currentArea.styles;
						area.hasConfig = gridItemHasConfig(currentArea.styles, currentArea.config);
					}

					//set editor permissions
					if (!area.allowed || area.allowAll === true) {
						area.$allowedEditors = $scope.availableEditors;
						area.$allowsRTE = true;
					} else {
						area.$allowedEditors = _.filter($scope.availableEditors, function (editor) {
							return _.indexOf(area.allowed, editor.alias) >= 0;
						});

						if (_.indexOf(area.allowed, "rte") >= 0) {
							area.$allowsRTE = true;
						}
					}

					//copy over existing controls into the new areas
					if (row.areas.length > areaIndex && row.areas[areaIndex].controls) {
						area.controls = currentArea.controls;

						_.forEach(area.controls, function (control, controlIndex) {
							$scope.initControl(control, controlIndex);
						});

					} else {
						//if empty
						area.controls = [];

						//if only one allowed editor
						if (area.$allowedEditors.length === 1) {
							$scope.addControl(area.$allowedEditors[0], area, 0, false);
						}
					}

					//set width
					area.$percentage = $scope.percentage(area.grid);
					area.$uniqueId = $scope.setUniqueId();

				} else {
					original.areas.splice(areaIndex, 1);
				}
			});

			//replace the old row
			original.$initialized = true;

			//set a disposable unique ID
			original.$uniqueId = $scope.setUniqueId();

			//set a no disposable unique ID (util for row styling)
			original.id = !row.id ? $scope.setUniqueId() : row.id;

			return original;
		}

	};


	// *********************************************
	// Init control
	// *********************************************

	$scope.initControl = function (control, index) {
		control.$index = index;
		control.$uniqueId = $scope.setUniqueId();

		//error handling in case of missing editor..
		//should only happen if stripped earlier
		if (!control.editor) {
			control.$editorPath = "views/propertyeditors/grid/editors/error.html";
		}

		if (!control.$editorPath) {
			var editorConfig = $scope.getEditor(control.editor.alias);

			if (editorConfig) {
				control.editor = editorConfig;

				//if its an absolute path
				if (control.editor.view.startsWith("/") || control.editor.view.startsWith("~/")) {
					control.$editorPath = umbRequestHelper.convertVirtualToAbsolutePath(control.editor.view);
				}
				else {
					//use convention
					control.$editorPath = "views/propertyeditors/grid/editors/" + control.editor.view + ".html";
				}
			}
			else {
				control.$editorPath = "views/propertyeditors/grid/editors/error.html";
			}
		}


	};


	gridService.getGridEditors().then(function (response) {
		$scope.availableEditors = response.data;

		//Localize the grid editor names
		$scope.availableEditors.forEach(function (value) {

			var a = 1;

		});

		$scope.contentReady = true;

		// *********************************************
		// Init grid
		// *********************************************

		eventsService.emit("uPersonalize.initializing", { scope: $scope, element: $element });

		$scope.initContent();

		eventsService.emit("uPersonalize.initialized", { scope: $scope, element: $element });

	});

	//Clean the grid value before submitting to the server, we don't need
	// all of that grid configuration in the value to be stored!! All of that
	// needs to be merged in at runtime to ensure that the real config values are used
	// if they are ever updated.
	var unsubscribe = $scope.$on("formSubmitting", function (e, args) {
		if (args.action === "save" && $scope.model.value && $scope.model.value.sections) {
			_.each($scope.model.value.sections, function (section) {
				if (section.rows) {
					_.each(section.rows, function (row) {
						if (row.areas) {
							_.each(row.areas, function (area) {

								//Remove the 'editors' - these are the allowed editors, these will
								// be injected at runtime to this editor, it should not be persisted

								if (area.editors) {
									delete area.editors;
								}

								if (area.controls) {
									_.each(area.controls, function (control) {
										if (control.editor) {
											//replace
											var alias = control.editor.alias;
											control.editor = {
												alias: alias
											};
										}
									});
								}
							});
						}
					});
				}
			});
		}
	});

	//when the scope is destroyed we need to unsubscribe
	$scope.$on("$destroy", function () {
		unsubscribe();
	});
});