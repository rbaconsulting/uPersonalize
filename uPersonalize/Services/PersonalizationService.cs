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
using System.Web;
using System.Collections.Generic;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

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

		public async Task<bool> OnPageLoad(int pageId)
		{
			if (pageId > 0)
			{
				await RecordPageLoad(pageId.ToString());
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
					await _cookieManager.SetCookie(PersonalizationConditions.Device_Type, deviceType.ToString());
				}
			}

			return true;
		}

		public async Task<bool> DoesFilterMatch(PersonalizationFilter filter)
		{
			var isMatch = false;

			if (filter != null && filter.IsValid())
			{
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
			}

			return isMatch;
		}

		public async Task<List<string>> ApplyFilterToGrid(List<string> attrs, PersonalizationFilter filter)
		{
			if (filter != null && filter.IsValid())
			{
				var isMatch = await DoesFilterMatch(filter);

				var personalizedValue = string.Empty;

				if (isMatch && filter.Action == PersonalizationActions.Show || !isMatch && filter.Action == PersonalizationActions.Hide)
				{
					personalizedValue = "uPersonalize-show";
				}
				else if (!isMatch && filter.Action == PersonalizationActions.Show || isMatch && filter.Action == PersonalizationActions.Hide)
				{
					personalizedValue = "uPersonalize-hide";
				}
				else if (isMatch && filter.Action == PersonalizationActions.Additional_Classes && !string.IsNullOrWhiteSpace(filter.AdditionalClasses))
				{
					personalizedValue = filter.AdditionalClasses;
				}

				if (!string.IsNullOrWhiteSpace(personalizedValue))
				{
					personalizedValue = HttpUtility.HtmlAttributeEncode(personalizedValue);

					var existingClasses = attrs.Find(a => a.StartsWith("class="));

					if (!string.IsNullOrWhiteSpace(existingClasses))
					{
						var i = attrs.IndexOf(existingClasses);
						attrs[i] = $"{existingClasses.TrimEnd('\"')} {personalizedValue}\"";
					}
					else
					{
						attrs.Add($"class=\"{personalizedValue}\"");
					}
				}
			}

			return attrs;
		}

		public async Task<bool> TriggerEvent(string eventName)
		{
			if (!string.IsNullOrWhiteSpace(eventName) && Regex.IsMatch(eventName, Events.Name))
			{
				return await _cookieManager.SetKeyValueListCookie(PersonalizationConditions.Event_Triggered, eventName);
			}

			return false;
		}

		public async Task<bool> RecordPageLoad(string pageId)
		{
			if (!string.IsNullOrWhiteSpace(pageId) && Regex.IsMatch(pageId, RegexRules.Umbraco.PageItemId))
			{
				return await _cookieManager.SetKeyValueListCookie(PersonalizationConditions.Visited_Page, pageId);
			}

			return false;
		}

		public async Task<int> GetTriggeredEventCount(string eventName)
		{
			if (!string.IsNullOrWhiteSpace(eventName) && Regex.IsMatch(eventName, Events.Name))
			{
				var cookieValue = await _cookieManager.GetCookie(PersonalizationConditions.Event_Triggered);

				if (!string.IsNullOrWhiteSpace(cookieValue) && cookieValue.Contains($"{eventName}:"))
				{
					var eventMatch = cookieValue.Split(',').FirstOrDefault(i => i.StartsWith($"{eventName}:"));

					if (!string.IsNullOrWhiteSpace(eventMatch))
					{
						var countString = eventMatch.Split(":")[1];

						return int.Parse(countString);
					}
				}
			}

			return -1;
		}

		public async Task<int> GetPageLoadCount(string pageId)
		{
			if (!string.IsNullOrWhiteSpace(pageId) && Regex.IsMatch(pageId, RegexRules.Umbraco.PageItemId))
			{
				var cookieValue = await _cookieManager.GetCookie(PersonalizationConditions.Visited_Page);

				if (!string.IsNullOrWhiteSpace(cookieValue) && cookieValue.Contains($"{pageId}:"))
				{
					var pageMatch = cookieValue.Split(',').FirstOrDefault(i => i.StartsWith($"{pageId}:"));

					if (!string.IsNullOrWhiteSpace(pageMatch))
					{
						var countString = pageMatch.Split(":")[1];

						return int.Parse(countString);
					}
				}
			}

			return -1;
		}
	}
}