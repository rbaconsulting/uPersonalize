angular.module("umbraco").controller("uPersonalizeSettingsController", function ($http, $scope, notificationsService) {
	$scope.showLoading = true;

	// could read positions from defaultConfig
	$scope.sameSiteOptions = [
		'Unspecified',
		'None',
		'Lax',
		'Strict'
	];

	$scope.model = {
		domain: '',
		secure: true,
		sameSite: $scope.sameSiteOptions[0],
		maxAgeDays: 365,
		maxAgeHours: 0,
		maxAgeMinutes: 0
	}

	getSettings();

	function getSettings() {
		makeRequest('/umbraco/backoffice/uPersonalize/PersonalizationBackoffice/GetPersonalizationSettings', 'GET', null, getSettingsCallback);
	}

	$scope.saveSettings = function () {
		$scope.showLoading = true;

		var data = {
			DomainCookieOption: $scope.model.domain,
			SecureCookieOption: $scope.model.secure,
			SameSiteCookieOption: $scope.model.sameSite,
			MaxAgeCookieOption: `${$scope.model.maxAgeDays}.${$scope.model.maxAgeHours}:${$scope.model.maxAgeMinutes}:00`
		};

		makeRequest('/umbraco/backoffice/uPersonalize/PersonalizationBackoffice/SavePersonalizationSettings', 'POST', data, saveSettingsCallback);
	}

	function getSettingsCallback(responseData) {
		var optionIndex = $scope.sameSiteOptions.indexOf(responseData.SameSiteCookieOption);

		if (optionIndex > -1) {
			$scope.model.sameSite = $scope.sameSiteOptions[optionIndex];
		}

		var daySplit = responseData.MaxAgeCookieOption.split('.');

		if (daySplit.length > 0) {
			$scope.model.maxAgeDays = parseInt(daySplit[0]);

			var hourMinuteSplit = daySplit[1].split(':');

			if (hourMinuteSplit.length > 0) {
				$scope.model.maxAgeHours = parseInt(hourMinuteSplit[0]);
				$scope.model.maxAgeMinutes = parseInt(hourMinuteSplit[1]);
			}
		}

		$scope.model.domain = responseData.DomainCookieOption;
		$scope.model.secure = responseData.SecureCookieOption;

		$scope.showLoading = false;
	}

	function saveSettingsCallback(responseData) {
		$scope.showLoading = false;
		notificationsService.success("uPersonalize", "Settings successfully saved!");
	}

	function errorCallback() {
		$scope.showLoading = false;
		notificationsService.error("uPersonalize", "Error encountered... please check the log viewer for more details.");
	}

	function makeRequest(url, method, data, callback) {
		$http({
			method: method,
			url: url,
			data: data
		}).then((response) => response.status == 200 ? callback(response.data) : errorCallback(), error => errorCallback());
	}
});