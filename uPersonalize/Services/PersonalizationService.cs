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
using Umbraco.Cms.Core.Security;
using static uPersonalize.Constants.RegexRules;
using Lucene.Net.Search;
using Umbraco.Cms.Web.Common;

namespace uPersonalize.Services
{
	public class PersonalizationService : IPersonalizationService
	{
		private readonly ILogger<PersonalizationService> _logger;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IPersonalizationCookieManager _cookieManager;
		private readonly IMemberManager _memberManager;

		public PersonalizationService(ILogger<PersonalizationService> logger, IMemberManager memberManager, IHttpContextAccessor httpContextAccessor, IPersonalizationCookieManager cookieManager)
		{
			_logger = logger;
			_httpContextAccessor = httpContextAccessor;
			_cookieManager = cookieManager;
			_memberManager = memberManager;
		}

		public async Task TrackUser(int pageId)
		{
			if (pageId > 0)
			{
				await TryPageVisit(pageId.ToString());
			}

			var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"];

			if (!StringValues.IsNullOrEmpty(userAgent))
			{
				var deviceType = DeviceTypes.Default;

				if (Regex.IsMatch(userAgent, UserAgents.Android))
				{
					deviceType = DeviceTypes.Android;
				}
				else if (Regex.IsMatch(userAgent, UserAgents.Windows))
				{
					deviceType = DeviceTypes.Windows;
				}

				if (deviceType != DeviceTypes.Default)
				{
					await _cookieManager.TrySetCookie(PersonalizationConditions.Device_Type, deviceType.ToString());
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
						var clientIPAddress = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress;

						if (Regex.IsMatch(filter.IpAddress, Conditions.IP) && !IPAddress.IsLoopback(clientIPAddress))
						{
							var ipToMatch = filter.IpAddress.Split('.');
							var ipAddressSegments = clientIPAddress.ToString().Split('.');

							for (int i = 0; i < 4; i++)
							{
								if (Regex.IsMatch(ipToMatch[i], Conditions.IPMask))
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
					var deviceType = await _cookieManager.GetCookie(filter.Condition);

					return !string.IsNullOrWhiteSpace(deviceType) && filter.DeviceToMatch.ToString().Equals(deviceType);
				case PersonalizationConditions.Visited_Page:
				case PersonalizationConditions.Visited_Page_Count:
				case PersonalizationConditions.Event_Triggered:
				case PersonalizationConditions.Event_Triggered_Count:
					var visitedPages = await _cookieManager.GetCookie(filter.Condition);
					var isEvent = filter.Condition == PersonalizationConditions.Event_Triggered || filter.Condition == PersonalizationConditions.Event_Triggered_Count;
					var compareTo = isEvent ? filter.EventName : filter.PageId;

					if (!string.IsNullOrWhiteSpace(visitedPages))
					{
						Parallel.ForEach(visitedPages.Split(','), pair =>
						{
							if (compareTo.Contains(pair.Split(':').FirstOrDefault()))
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
				case PersonalizationConditions.Logged_In:
					return _memberManager.IsLoggedIn();
				default:
					break;
			}

			return isMatch;
		}

		public async Task<bool> TryTriggerEvent(string eventName)
		{
			if (!string.IsNullOrWhiteSpace(eventName) && Regex.IsMatch(eventName, Events.Name))
			{
				return await _cookieManager.TrySetKeyValueListCookie(PersonalizationConditions.Event_Triggered, eventName);
			}

			return false;
		}

		public async Task<bool> TryPageVisit(string pageId)
		{
			if (!string.IsNullOrWhiteSpace(pageId) && Regex.IsMatch(pageId, RegexRules.Umbraco.PageItemId))
			{
				return await _cookieManager.TrySetKeyValueListCookie(PersonalizationConditions.Visited_Page, pageId);
			}

			return false;
		}

		public async Task<int> GetTriggeredEventCount(string eventName)
		{
			if (!string.IsNullOrWhiteSpace(eventName) && Regex.IsMatch(eventName, Events.Name))
			{
				var cookieValue = await _cookieManager.GetCookie(PersonalizationConditions.Event_Triggered);
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
			if (!string.IsNullOrWhiteSpace(pageId) && Regex.IsMatch(pageId, RegexRules.Umbraco.PageItemId))
			{
				var cookieValue = await _cookieManager.GetCookie(PersonalizationConditions.Visited_Page);

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