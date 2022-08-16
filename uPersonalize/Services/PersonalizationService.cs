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
using System.Web;
using System.Collections.Generic;

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

		public async Task<bool> IsOptOut()
		{
			return await _cookieManager.IsOptOut();
		}

		public async Task<bool> OptOut()
		{
			await _cookieManager.DeleteCookies();
			await _cookieManager.SetOptOut();

			return false;
		}

		public async Task<bool> ResetPersonalization(bool includeOptOut = false)
		{
			await _cookieManager.DeleteCookies(includeOptOut);

			return false;
		}

		public async Task<bool> OnPageLoad(int pageId)
		{
			if (!await IsOptOut())
			{
				if (pageId > 0)
				{
					await RecordPageLoad(pageId.ToString());
				}

				var userAgent = _httpContextAccessor.HttpContext.Request.Headers[Headers.UserAgent.Name];

				if (!StringValues.IsNullOrEmpty(userAgent))
				{
					var deviceType = DeviceTypes.Default;

					if (Headers.UserAgent.RegexRules.Android.IsMatch(userAgent))
					{
						deviceType = DeviceTypes.Android;
					}
					else if (Headers.UserAgent.RegexRules.Windows.IsMatch(userAgent))
					{
						deviceType = DeviceTypes.Windows;
					}

					if (deviceType != DeviceTypes.Default)
					{
						await _cookieManager.SetCookie(PersonalizationConditions.Device_Type, deviceType.ToString());
					}
				}
			}

			return true;
		}

		public async Task<bool> DoesFilterMatch(PersonalizationFilter filter)
		{
			var isMatch = false;

			if (!await IsOptOut() && filter != null && filter.IsValid())
			{
				switch (filter.Condition)
				{
					case PersonalizationConditions.IP_Address:
						if (!string.IsNullOrWhiteSpace(filter.IpAddress))
						{
							var clientIPAddress = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress;

							if (Headers.XForwardedFor.RegexRules.Ip.IsMatch(filter.IpAddress) && !IPAddress.IsLoopback(clientIPAddress))
							{
								var ipToMatch = filter.IpAddress.Split('.');
								var ipAddressSegments = clientIPAddress.ToString().Split('.');

								for (int i = 0; i < 4; i++)
								{
									if (Headers.XForwardedFor.RegexRules.IpMask.IsMatch(ipToMatch[i]))
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
					case PersonalizationConditions.DateTime_Before:
						return DateTime.Now.CompareTo(filter.DateTimeCompare) < 0;
					case PersonalizationConditions.DateTime_After:
						return DateTime.Now.CompareTo(filter.DateTimeCompare) > 0;
					default:
						break;
				}
			}

			return isMatch;
		}

		public async Task<List<string>> ApplyFilterToGrid(List<string> attrs, PersonalizationFilter filter)
		{
			if (!await IsOptOut() && filter != null && filter.IsValid())
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
			if (!await IsOptOut() && !string.IsNullOrWhiteSpace(eventName) && Cookies.ClickedEvents.RegexRules.EventName.IsMatch(eventName))
			{
				return await _cookieManager.SetKeyValueListCookie(PersonalizationConditions.Event_Triggered, eventName);
			}

			return false;
		}

		public async Task<bool> RecordPageLoad(string pageId)
		{
			

			if (!await IsOptOut() && !string.IsNullOrWhiteSpace(pageId) && Cookies.VisitedPages.RegexRules.PageItemId.IsMatch(pageId))
			{
				return await _cookieManager.SetKeyValueListCookie(PersonalizationConditions.Visited_Page, pageId);
			}

			return false;
		}

		public async Task<int> GetTriggeredEventCount(string eventName)
		{
			if (!await IsOptOut() && !string.IsNullOrWhiteSpace(eventName) && Cookies.ClickedEvents.RegexRules.EventName.IsMatch(eventName))
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

			return 0;
		}

		public async Task<int> GetPageLoadCount(string pageId)
		{
			if (!await IsOptOut() && !string.IsNullOrWhiteSpace(pageId) && Cookies.VisitedPages.RegexRules.PageItemId.IsMatch(pageId))
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

			return 0;
		}
	}
}