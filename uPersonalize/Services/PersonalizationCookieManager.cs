using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using uPersonalize.Enums;
using uPersonalize.Enums.Extensions;
using uPersonalize.Interfaces;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

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

		public async Task<string> GetCookie(PersonalizationConditions type)
		{
			return await Task.Run(() =>
            {
				var cookieName = type.GetCookieName();

				return string.IsNullOrWhiteSpace(cookieName) ? string.Empty :
					   HttpContextAccessor.HttpContext.Request.Cookies.TryGetValue(cookieName, out string cookieValue) ? cookieValue : string.Empty;
			});
		}

		public async Task<bool> SetCookie(PersonalizationConditions type, string cookieValue)
		{
			return await Task.Run(() =>
			{
				if (!string.IsNullOrWhiteSpace(cookieValue))
				{
					var cookieName = type.GetCookieName();

					if (!string.IsNullOrWhiteSpace(cookieName))
					{
						HttpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, cookieValue, _cookieOptions);
						return true;
					}
				}

				return false;
			});
		}

		public async Task<bool> SetKeyValueListCookie(PersonalizationConditions type, string key, int value = 1)
		{
			if (!string.IsNullOrWhiteSpace(key) && (int)type > 2)
			{
				var currentCookieValue = await GetCookie(type);

				if (!string.IsNullOrWhiteSpace(currentCookieValue))
				{
					if (currentCookieValue.Contains($"{key}:"))
					{
						var currentValue = currentCookieValue.Split(',').FirstOrDefault(i => i.StartsWith($"{key}:"));
						var parseResult = int.TryParse(currentValue.Split(':')[1], out int currentCount);

						if (parseResult)
						{
							value += currentCount;
							return await SetCookie(type, currentCookieValue.Replace(currentValue, $"{key}:{value}"));
						}
						else
						{
							return await SetCookie(type, $"{currentCookieValue},{key}:{value}");
						}
					}
					else
					{
						return await SetCookie(type, $"{currentCookieValue},{key}:{value}");
					}
				}
				else
				{
					return await SetCookie(type, $"{key}:{value}");
				}
			}

			return false;
		}
	}
}