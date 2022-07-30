angular.module("umbraco").controller("uPersonalizeButtonController", function ($scope, localizationService, editorService) {
	var config = $scope.model.value ? JSON.parse($scope.model.value) : {
		condition: '',
		action: '',
		ipAddress: '',
		deviceToMatch: 0,
		pageId: '',
		eventName: '',
		pageEventCount: 0,
		dateTimeCompare: '',
		additionalClasses: ''
	};

	if (config) {

		config.dateTimeCompare = config.dateTimeCompare.replace('Z', '');

		$scope.editGridItemSettings = function () {
			var dialogOptions = {
				title: "uPersonalize",
				view: "/App_Plugins/uPersonalize/dialogs/uPersonalizeDialog.html",
				size: "small",
				uPersonalizeConfig: config,
				submit: function (model) {
					$scope.model.value = JSON.stringify(model);

					editorService.close();
				},
				close: function () {
					editorService.close();
				}
			};

			editorService.open(dialogOptions);
		};
	}
});