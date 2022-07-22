using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using uPersonalize.Enums;
using uPersonalize.Enums.Extensions;
using uPersonalize.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace uPersonalize.Services
{
	public class PersonalizationCookieManager : IPersonalizationCookieManager
	{
		private readonly string _optOutCookieName = "uPersonalize_opt_out";
		private readonly CookieOptions _cookieOptions;
		private readonly ILogger<PersonalizationCookieManager> _logger;

		private IHttpContextAccessor HttpContextAccessor { get; }

		public PersonalizationCookieManager(ILogger<PersonalizationCookieManager> logger, IHttpContextAccessor httpContextAccessor, IPersonalizationSettings personalizationSettings)
		{
			_logger = logger;
			_cookieOptions = personalizationSettings.GetCookieOptions();

			HttpContextAccessor = httpContextAccessor;
		}

		public async Task<bool> IsOptOut()
		{
			return await Task.Run(() =>
			{
				try
				{
					if(HttpContextAccessor.HttpContext.Request.Cookies.ContainsKey(_optOutCookieName))
					{
						HttpContextAccessor.HttpContext.Request.Cookies.TryGetValue(_optOutCookieName, out string cookieStringValue);

						var parseResult = bool.TryParse(cookieStringValue, out bool cookieValue);

						return parseResult && cookieValue;
					}
				}
				catch (Exception e)
				{
					_logger.LogError(e.Message);
				}

				return false;
			});
		}

		public async Task<bool> SetOptOut()
		{
			return await Task.Run(() =>
			{
				try
				{
					HttpContextAccessor.HttpContext.Response.Cookies.Append(_optOutCookieName, "True", _cookieOptions);

					return true;
				}
				catch (Exception e)
				{
					_logger.LogError(e.Message);
				}

				return false;
			});
		}

		public async Task<bool> DeleteCookies(bool includeOptOut = false)
		{
			return await Task.Run(() =>
			{
				try
				{
					foreach (PersonalizationConditions personalizationCondition in Enum.GetValues(typeof(PersonalizationConditions)))
					{
						var cookieName = personalizationCondition.GetCookieName();

						HttpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);
					}

					if(includeOptOut)
					{
						HttpContextAccessor.HttpContext.Response.Cookies.Delete(_optOutCookieName);
					}

					return true;
				}
				catch(Exception e)
				{
					_logger.LogError(e.Message);
				}

				return false;
			});
		}

		public async Task<string> GetCookie(PersonalizationConditions type)
		{
			return await Task.Run(() =>
            {
				try
				{
					var cookieName = type.GetCookieName();

					return string.IsNullOrWhiteSpace(cookieName) ? string.Empty :
						   HttpContextAccessor.HttpContext.Request.Cookies.TryGetValue(cookieName, out string cookieValue) ? cookieValue : string.Empty;
				}
				catch (Exception e)
				{
					_logger.LogError(e.Message);
				}

				return string.Empty;
			});
		}

		public async Task<bool> SetCookie(PersonalizationConditions type, string cookieValue)
		{
			return await Task.Run(() =>
			{
				try
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
				}
				catch (Exception e)
				{
					_logger.LogError(e.Message);
				}

				return false;
			});
		}

		public async Task<bool> SetKeyValueListCookie(PersonalizationConditions type, string key, int value = 1)
		{
			try
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
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
			}

			return false;
		}
	}
}