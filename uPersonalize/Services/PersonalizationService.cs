using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using uPersonalize.Enums;
using uPersonalize.Interfaces;
using uPersonalize.Models;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using uPersonalize.Constants;
using Microsoft.Extensions.Primitives;

namespace uPersonalize.Services
{
	public class PersonalizationService : IPersonalizationService
	{
		private ILogger<PersonalizationService> Logger { get; }
		private IHttpContextAccessor HttpContextAccessor { get; }
		private IPersonalizationCookieManager CookieManager { get; }

		public PersonalizationService(ILogger<PersonalizationService> logger, IHttpContextAccessor httpContextAccessor, IPersonalizationCookieManager cookieManager)
		{
			Logger = logger;
			HttpContextAccessor = httpContextAccessor;

			CookieManager = cookieManager;
		}

		public async Task TrackUser(int pageId)
		{
			if (pageId > 0)
			{
				await TryPageVisit(pageId.ToString());
			}

			var userAgent = HttpContextAccessor.HttpContext.Request.Headers["User-Agent"];

			if (!StringValues.IsNullOrEmpty(userAgent))
			{
				var deviceType = DeviceTypes.Default;

				if (Regex.IsMatch(userAgent, "Android"))
				{
					deviceType = DeviceTypes.Android;
				}
				else if (Regex.IsMatch(userAgent, "Windows"))
				{
					deviceType = DeviceTypes.Desktop_Windows;
				}

				if (deviceType != DeviceTypes.Default)
				{
					await CookieManager.TrySetCookie(PersonalizationConditions.Device_Type, deviceType.ToString());
				}
			}
		}

		public async Task<bool> IsMatch(PersonalizationFilter filter)
		{
			var isMatch = false;

			switch (filter.Condition)
			{
				case PersonalizationConditions.IP_Address:
					if (!string.IsNullOrWhiteSpace(filter.IpAddress))
					{
						var regex = new Regex("^((\\d|(x|X)){3}\\.?){4}$");
						var regexMask = new Regex("(x|X){3}");

						var ipAddress = HttpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty;

						if (regex.IsMatch(filter.IpAddress) && !IPAddress.IsLoopback(HttpContextAccessor.HttpContext.Connection.RemoteIpAddress))
						{
							var ipToMatch = filter.IpAddress.Split('.');
							var ipAddressSegments = ipAddress.Split('.');

							for (int i = 0; i < 4; i++)
							{
								if (regexMask.IsMatch(ipToMatch[i]))
								{
									continue;
								}
								else if (ipToMatch[i].Equals(ipAddressSegments[i], StringComparison.OrdinalIgnoreCase))
								{
									isMatch = true;
								}
								else
								{
									return false;
								}
							}
						}
					}

					break;
				case PersonalizationConditions.Device_Type:
					var deviceType = await CookieManager.GetCookie(filter.Condition);

					return !string.IsNullOrWhiteSpace(deviceType) ? filter.DeviceToMatch.ToString().Equals(deviceType) : false;
				case PersonalizationConditions.Visited_Page:
				case PersonalizationConditions.Visited_Page_Count:
				case PersonalizationConditions.Event_Triggered:
				case PersonalizationConditions.Event_Triggered_Count:
					var visitedPages = await CookieManager.GetCookie(filter.Condition);
					var isEvent = filter.Condition == PersonalizationConditions.Event_Triggered || filter.Condition == PersonalizationConditions.Event_Triggered_Count;
					var compareTo = isEvent ? filter.EventName : filter.PageId;

					if (!string.IsNullOrWhiteSpace(visitedPages))
					{
						Parallel.ForEach(visitedPages.Split(','), pair =>
						{
							if (!string.IsNullOrWhiteSpace(pair) && pair.StartsWith(compareTo))
							{
								if (filter.Condition == PersonalizationConditions.Visited_Page_Count || filter.Condition == PersonalizationConditions.Event_Triggered_Count)
								{
									var count = pair.Split(':').LastOrDefault();

									if (!string.IsNullOrWhiteSpace(count))
									{
										var parseResult = int.TryParse(count, out int cookieCount);

										if (parseResult && cookieCount >= filter.PageEventCount)
										{
											isMatch = true;
										}
									}
								}
								else
								{
									isMatch = true;
								}
							}
						});
					}

					break;
				default:
					break;
			}

			return isMatch;
		}

		public async Task<bool> TryTriggerEvent(string eventName)
		{
			if (!string.IsNullOrWhiteSpace(eventName) && RegexRules.Events.Name.IsMatch(eventName))
			{
				return await CookieManager.TrySetKeyValueListCookie(PersonalizationConditions.Event_Triggered, eventName);
			}

			return false;
		}

		public async Task<bool> TryPageVisit(string pageId)
		{
			if (!string.IsNullOrWhiteSpace(pageId) && RegexRules.Umbraco.PageItemId.IsMatch(pageId))
			{
				return await CookieManager.TrySetKeyValueListCookie(PersonalizationConditions.Visited_Page, pageId);
			}

			return false;
		}

		public async Task<int> GetTriggeredEventCount(string eventName)
		{
			if (!string.IsNullOrWhiteSpace(eventName) && RegexRules.Events.Name.IsMatch(eventName))
			{
				var cookieValue = await CookieManager.GetCookie(PersonalizationConditions.Event_Triggered);
				var regex = new Regex($"{eventName}:\\d*");

				if (!string.IsNullOrWhiteSpace(cookieValue))
				{
					var match = regex.Match(cookieValue);

					if (match.Success)
					{
						var countString = match.Value.Split(":")[1];

						return int.Parse(countString);
					}
				}
			}

			return -1;
		}

		public async Task<int> GetPageVisitCount(string pageId)
		{
			if (!string.IsNullOrWhiteSpace(pageId) && RegexRules.Umbraco.PageItemId.IsMatch(pageId))
			{
				var cookieValue = await CookieManager.GetCookie(PersonalizationConditions.Visited_Page);

				var regex = new Regex($"{pageId}:\\d*");

				if (!string.IsNullOrWhiteSpace(cookieValue))
				{
					var match = regex.Match(cookieValue);

					if (match.Success)
					{
						var countString = match.Value.Split(":")[1];

						return int.Parse(countString);
					}
				}
			}

			return -1;
		}
	}
}