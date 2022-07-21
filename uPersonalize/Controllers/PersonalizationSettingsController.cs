using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using uPersonalize.Interfaces;
using System;
using System.Net;
using System.Threading.Tasks;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using uPersonalize.Models.Requests;
using uPersonalize.Constants;

namespace uPersonalize.Controllers
{
	/// <summary>
	/// /umbraco/backoffice/uPersonalize/PersonalizationSettings/{action}
	/// </summary>
	[PluginController(AppPlugin.Name)]
	public class PersonalizationSettingsController : UmbracoAuthorizedApiController
	{
		private ILogger<PersonalizationSettingsController> Logger { get; }
		private IPersonalizationSettings _uPersonalizeSettings { get; set; }

		public PersonalizationSettingsController(ILogger<PersonalizationSettingsController> logger, IPersonalizationSettings uPersonalizeSettings)
		{
			Logger = logger;
			_uPersonalizeSettings = uPersonalizeSettings;
		}

		[HttpGet]
		public IPersonalizationSettings GetPersonalizationSettings()
		{
			return _uPersonalizeSettings;
		}

		[HttpPost]
		public async Task<IActionResult> SavePersonalizationSettings([FromBody] SavePersonalizationSettings personalizationSettings)
{
			try
			{
				_uPersonalizeSettings.SecureCookieOption = personalizationSettings.SecureCookieOption;

				await _uPersonalizeSettings.Save();

				return Ok();
			}
			catch(Exception e)
			{
				Logger.LogError(e, e.Message);
				return StatusCode((int)HttpStatusCode.InternalServerError);
			}
		}
	}
}