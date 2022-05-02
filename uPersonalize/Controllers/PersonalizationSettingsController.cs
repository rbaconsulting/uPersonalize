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
		private IPersonalizationSettings PersonalizationSettings { get; }

		public PersonalizationSettingsController(ILogger<PersonalizationSettingsController> logger, IPersonalizationSettings personalizationSettings)
		{
			Logger = logger;
			PersonalizationSettings = personalizationSettings;
		}

		[HttpGet]
		public IPersonalizationSettings GetPersonalizationSettings()
		{
			return PersonalizationSettings;
		}

		[HttpPost]
		public async Task<IActionResult> SavePersonalizationSettings([FromBody] PersonalizationSettings personalizationSettings)
		{
			try
			{
				PersonalizationSettings.Domain = personalizationSettings?.Domain ?? string.Empty;
				PersonalizationSettings.Secure = personalizationSettings?.Secure ?? false;
				PersonalizationSettings.SameSite = personalizationSettings?.SameSite ?? SameSiteMode.Unspecified;
				PersonalizationSettings.MaxAge = personalizationSettings?.MaxAge ?? TimeSpan.FromDays(365);

				await PersonalizationSettings.Save();

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