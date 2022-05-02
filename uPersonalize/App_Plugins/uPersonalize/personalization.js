const makeRequest = (url, method, data = null) => {
	const response = fetch(url, {
		method: method,
		body: data
	});

	return response;
};

function visitPage(pageId) {
	makeRequest(`/umbraco/uPersonalize/Personalization/pageVisit/${pageId}`, 'POST');
};

window.addEventListener('load', function () {
	var buttons = this.document.getElementsByClassName('uPersonalizeButton');

	if (buttons.length > 0) {
		for (var counter = 0; counter < buttons.length; counter++) {
			buttons[counter].addEventListener("click", function () {
				makeRequest(`/umbraco/uPersonalize/Personalization/TriggerEvent/testEvent`, 'POST');
			});
		}
	}

	//var pageId = pageIdTag.getAttribute('content');

	//visitPage(pageId);
})