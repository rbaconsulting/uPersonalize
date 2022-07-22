class uPersonalize {
	static async makeRequest(url, method, data) {
		let response = await fetch(url, {
			method: method,
			body: data
		}).then(response => {
			return response.text();
		});

		return response;
	}

	static async isOptOut() {
		let result = await uPersonalize.makeRequest('/umbraco/uPersonalize/Personalization/isOptOut', 'GET', null);

		return result;
	}

	static async optOut() {
		await uPersonalize.makeRequest('/umbraco/uPersonalize/Personalization/optOut', 'POST', null);
	}

	static async resetPersonalization() {
		await uPersonalize.makeRequest('/umbraco/uPersonalize/Personalization/resetPersonalization', 'POST', null);
	}

	static async onPageLoad(pageId) {
		await uPersonalize.makeRequest(`/umbraco/uPersonalize/Personalization/onPageLoad/${pageId}`, 'POST', null);
	}

	static async doesFilterMatch(personalizationFilter, cssSelector, additionalClasses) {
		let isMatch = await uPersonalize.makeRequest(`/umbraco/uPersonalize/Personalization/doesFilterMatch`, 'POST', personalizationFilter);

		if (isMatch) {
			var elements = document.querySelectorAll(cssSelector);

			Array.from(elements, element => {
				switch (personalizationFilter.Action) {
					case 0:
						return;
					case 1:
						element.classList.add('uPersonalize-hide');
						element.classList.remove('uPersonalize-show');

						break;
					case 2:
						element.classList.add('uPersonalize-show');
						element.classList.remove('uPersonalize-hide');

						break;
					case 3:
						element.classList.add(additionalClasses);

						break;
					default:
						return;
				}
			});

			for (var i = 0; i < elements.length; i++) {

				var element = elements[i];

				element.add
			}


		}
	}

	static async triggerEvent(eventName) {
		await uPersonalize.makeRequest(`/umbraco/uPersonalize/Personalization/triggerEvent/${eventName}`, 'POST', null);
	}

	static async recordPageLoad(pageId) {
		await uPersonalize.makeRequest(`/umbraco/uPersonalize/Personalization/recordPageLoad/${pageId}`, 'POST', null);
	}

	static async getTriggeredEventCount(eventName) {
		let eventCount = await uPersonalize.makeRequest(`/umbraco/uPersonalize/Personalization/getTriggeredEventCount/${eventName}`, 'GET', null);

		return eventCount;
	}

	static async getPageLoadCount(pageId) {
		let pageCount = await uPersonalize.makeRequest(`/umbraco/uPersonalize/Personalization/getPageLoadCount/${pageId}`, 'GET', null);

		return pageCount;
	}

	static initEventButtons() {
		var buttons = document.getElementsByClassName('uPersonalizeButton', null);

		if (buttons.length > 0) {
			for (var i = 0; i < buttons.length; i++) {

				var button = buttons[i];

				var eventName = button.dataset.upersonalizeEventName;

				if (eventName) {
					button.addEventListener("click", async function () {
						await uPersonalize.triggerEvent(eventName);
					});
				}
			}
		}
	}
}

window.addEventListener('load', function () {
	uPersonalize.initEventButtons();
});