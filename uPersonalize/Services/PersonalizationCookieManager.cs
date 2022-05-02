using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using uPersonalize.Enums;
using uPersonalize.Enums.Extensions;
using uPersonalize.Interfaces;
using System.Linq;
using System.Text.RegularExpressions;

namespace uPersonalize.Services
{
	public class PersonalizationCookieManager : IPersonalizationCookieManager
	{
		private readonly CookieOptions _cookieOptions;

		private ILogger<PersonalizationCookieManager> Logger { get; }
		private IHttpContextAccessor HttpContextAccessor { get; }

		public PersonalizationCookieManager(ILogger<PersonalizationCookieManager> logger, IHttpContextAccessor httpContextAccessor, IPersonalizationSettings personalizationSettings)
		{
			Logger = logger;
			HttpContextAccessor = httpContextAccessor;

			_cookieOptions = personalizationSettings.GetCookieOptions();
		}

		public string GetPersonalizationCookie(PersonalizationConditions type)
		{
			var cookieName = type.GetCookieName();

			return HttpContextAccessor.HttpContext.Request.Cookies.TryGetValue(cookieName, out string cookieValue) ? cookieValue : string.Empty;
		}

		public void SetPersonalizationCookie(PersonalizationConditions type, string cookieValue)
		{
			var cookieName = type.GetCookieName();

			HttpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, cookieValue, _cookieOptions);
		}

		public void SetPairValueListCookie(PersonalizationConditions type, string key, int value = 1)
		{
			var currentCookieValue = GetPersonalizationCookie(type);

			if (!string.IsNullOrWhiteSpace(currentCookieValue))
			{
				var regex = new Regex($"{key}:\\d*");
				var match = regex.Match(currentCookieValue);

				if (match.Success)
				{
					var parseResult = int.TryParse(match.Value.Split(':')[1], out int currentCount);

					if (parseResult)
					{
						value += currentCount;
						SetPersonalizationCookie(type, regex.Replace(currentCookieValue, $"{key}:{value}"));
					}
					else
					{
						SetPersonalizationCookie(type, $"{currentCookieValue},{key}:{value}");
					}
				}
				else
				{
					SetPersonalizationCookie(type, $"{currentCookieValue},{key}:{value}");
				}
			}
			else
			{
				SetPersonalizationCookie(type, $"{key}:{value}");
			}
		}
	}
}