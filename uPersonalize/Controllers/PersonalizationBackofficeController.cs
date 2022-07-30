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
	/// /umbraco/backoffice/uPersonalize/PersonalizationBackoffice/{action}
	/// </summary>
	[PluginController(AppPlugin.Name)]
	public class PersonalizationBackofficeController : UmbracoAuthorizedApiController
	{
		private ILogger<PersonalizationBackofficeController> Logger { get; }
		private IPersonalizationSettings _uPersonalizeSettings { get; set; }

		public PersonalizationBackofficeController(ILogger<PersonalizationBackofficeController> logger, IPersonalizationSettings uPersonalizeSettings)
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