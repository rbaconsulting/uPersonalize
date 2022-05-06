using Microsoft.AspNetCore.Mvc;
using uPersonalize.Interfaces;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Controllers;

namespace uPersonalize.Controllers
{
	/// <summary>
	/// ~/Umbraco/uPersonalize/Personalization
	/// </summary>
	[PluginController("uPersonalize")]
	public class PersonalizationController : UmbracoApiController
	{
		private readonly IPersonalizationService _personalizationService;

		public PersonalizationController(IPersonalizationService personalizationService)
		{
			_personalizationService = personalizationService;
		}

		/// <summary>
		/// ~/Umbraco/uPersonalize/Personalization/PageVisit
		/// </summary>
		[HttpPost]
		[Route("umbraco/uPersonalize/Personalization/TriggerEvent/{eventName}")]
		public void TriggerEvent(string eventName)
		{
			if (!string.IsNullOrWhiteSpace(eventName))
			{
				_personalizationService.TryTriggerEvent(eventName);
			}
		}

		/// <summary>
		/// ~/Umbraco/uPersonalize/Personalization/PageVisit
		/// </summary>
		[HttpPost]
		[Route("umbraco/uPersonalize/Personalization/PageVisit/{pageId}")]
		public void PageVisit(string pageId)
		{
			if (!string.IsNullOrWhiteSpace(pageId))
			{
				_personalizationService.TryPageVisit(pageId);
			}
		}
	}
}