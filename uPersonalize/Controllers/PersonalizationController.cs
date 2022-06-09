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
		[Route("umbraco/uPersonalize/Personalization/TrackUser/{pageId}")]
		public async Task TrackUser(int pageId)
		{
			await _personalizationService.TrackUser(pageId);
		}

		[HttpPost]
		[Route("umbraco/uPersonalize/Personalization/IsMatch")]
		public async Task<bool> IsMatch([FromBody] PersonalizationFilter filter)
		{
			return await _personalizationService.IsMatch(filter);
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
				await _personalizationService.TryTriggerEvent(eventName);
			}
		}

		/// <summary>
		/// ~/Umbraco/uPersonalize/Personalization/PageVisit
		/// </summary>
		[HttpPost]
		[Route("umbraco/uPersonalize/Personalization/PageVisit/{pageId}")]
		public async Task PageVisit(string pageId)
		{
			if (!string.IsNullOrWhiteSpace(pageId))
			{
				await _personalizationService.TryPageVisit(pageId);
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
		[Route("umbraco/uPersonalize/Personalization/GetPageVisitCount/{pageId}")]
		public async Task<int> GetPageVisitCount(string pageId)
		{
			return await _personalizationService.GetPageVisitCount(pageId);
		}
	}
}