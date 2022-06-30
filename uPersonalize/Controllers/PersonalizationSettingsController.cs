using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using uPersonalize.Interfaces;
using uPersonalize.Models;
using System;
using System.Net;
using System.Threading.Tasks;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace uPersonalize.Controllers
{
	/// <summary>
	/// /umbraco/backoffice/uPersonalize/PersonalizationSettings/{action}
	/// </summary>
	[PluginController("uPersonalize")]
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
		public async Task<IActionResult> SavePersonalizationSettings([FromBody] PersonalizationSettings personalizationSettings)
		{
			try
			{
				_uPersonalizeSettings = PersonalizationSettings.Create(personalizationSettings?.Domain, personalizationSettings.Secure, personalizationSettings.SameSite, personalizationSettings.MaxAge);

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