using Microsoft.AspNetCore.Mvc;
using uPersonalize.Interfaces;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Controllers;
using System.Threading.Tasks;
using uPersonalize.Models;

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

		[HttpPost]
		[Route("umbraco/uPersonalize/Personalization/OnPageLoad/{pageId}")]
		public async Task OnPageLoad(int pageId)
		{
			await _personalizationService.OnPageLoad(pageId);
		}

		[HttpPost]
		[Route("umbraco/uPersonalize/Personalization/DoesFilterMatch")]
		public async Task<bool> DoesFilterMatch([FromBody] PersonalizationFilter filter)
		{
			return await _personalizationService.DoesFilterMatch(filter);
		}

		/// <summary>
		/// ~/Umbraco/uPersonalize/Personalization/PageVisit
		/// </summary>
		[HttpPost]
		[Route("umbraco/uPersonalize/Personalization/TriggerEvent/{eventName}")]
		public async Task TriggerEvent(string eventName)
		{
			if (!string.IsNullOrWhiteSpace(eventName))
			{
				await _personalizationService.TriggerEvent(eventName);
			}
		}

		/// <summary>
		/// ~/Umbraco/uPersonalize/Personalization/PageVisit
		/// </summary>
		[HttpPost]
		[Route("umbraco/uPersonalize/Personalization/RecordPageLoad/{pageId}")]
		public async Task RecordPageLoad(string pageId)
		{
			if (!string.IsNullOrWhiteSpace(pageId))
			{
				await _personalizationService.RecordPageLoad(pageId);
			}
		}

		/// <summary>
		/// ~/Umbraco/uPersonalize/Personalization/PageVisit
		/// </summary>
		[HttpGet]
		[Route("umbraco/uPersonalize/Personalization/GetTriggeredEventCount/{eventName}")]
		public async Task<int> GetTriggeredEventCount(string eventName)
		{
			return await _personalizationService.GetTriggeredEventCount(eventName);
		}

		/// <summary>
		/// ~/Umbraco/uPersonalize/Personalization/PageVisit
		/// </summary>
		[HttpGet]
		[Route("umbraco/uPersonalize/Personalization/GetPageLoadCount/{pageId}")]
		public async Task<int> GetPageLoadCount(string pageId)
		{
			return await _personalizationService.GetPageLoadCount(pageId);
		}
	}
}