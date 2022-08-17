angular.module("umbraco").controller("uPersonalizeButtonController", function ($scope, editorService) {
	$scope.editGridItemSettings = function () {
		var dialogOptions = {
			title: "uPersonalize",
			view: "/App_Plugins/uPersonalize/dialogs/uPersonalizeDialog.html",
			size: "small",
			uPersonalizeConfig: config,
			submit: function (model) {
				$scope.model.value = JSON.stringify(model);
				$scope.setView();
				editorService.close();
			},
			close: function () {
				editorService.close();
			}
		};

		editorService.open(dialogOptions);
	};

	$scope.removePersonalization = function () {
		$scope.model.value = null;
		$scope.setView();
	};

	$scope.setView = function () {
		if ($scope.model.value) {
			vm.buttonText = 'Edit Personalization';
			vm.showDeleteButton = true;
		}
		else {
			vm.buttonText = 'Personalize';
			vm.showDeleteButton = false;
		}
	};

	var vm = this;
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

	$scope.setView();

	if (config) {
		if (config.dateTimeCompare) {
			config.dateTimeCompare = config.dateTimeCompare.replace('Z', '');
		}
	}
});