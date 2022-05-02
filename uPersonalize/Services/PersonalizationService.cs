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
			var deviceType = DeviceTypes.Default;
			var userAgent = HttpContextAccessor.HttpContext.Request.Headers["User-Agent"];

			if(Regex.IsMatch(userAgent, "Android"))
            {
				deviceType = DeviceTypes.Android;
			}
			else if (Regex.IsMatch(userAgent, "Windows"))
			{
				deviceType = DeviceTypes.Desktop_Windows;
			}

			if (deviceType != DeviceTypes.Default)
            {
				CookieManager.SetPersonalizationCookie(PersonalizationConditions.Device_Type, deviceType.ToString());
			}

			if (pageId > 0)
            {
				PageVisited(pageId.ToString());
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

						var ipAddress = HttpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

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
					var deviceType = CookieManager.GetPersonalizationCookie(filter.Condition);

					return !string.IsNullOrWhiteSpace(deviceType) ? filter.DeviceToMatch.ToString().Equals(deviceType) : false;
				case PersonalizationConditions.Visited_Page:
				case PersonalizationConditions.Visited_Page_Count:
				case PersonalizationConditions.Event_Triggered:
				case PersonalizationConditions.Event_Triggered_Count:
					var visitedPages = CookieManager.GetPersonalizationCookie(filter.Condition);
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

										if (parseResult && cookieCount > filter.PageEventCount)
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

		public void TriggerEvent(string eventName)
		{
			if (!string.IsNullOrWhiteSpace(eventName))
			{
				CookieManager.SetPairValueListCookie(PersonalizationConditions.Event_Triggered, eventName);
			}
		}

		public void PageVisited(string pageId)
		{
			if (!string.IsNullOrWhiteSpace(pageId))
			{
				CookieManager.SetPairValueListCookie(PersonalizationConditions.Visited_Page, pageId);
			}
		}

		public int GetTriggeredEventCount(string eventName)
		{
			if (!string.IsNullOrWhiteSpace(eventName))
			{
				var cookieValue = CookieManager.GetPersonalizationCookie(PersonalizationConditions.Event_Triggered);

				var regex = new Regex($"{eventName}:\\d*");
				var match = regex.Match(cookieValue);

				if(match.Success)
				{
					var countString = match.Value.Split(":")[1];

					return int.Parse(countString);
				}
			}

			return 0;
		}

		public int GetPageVisitCount(string pageId)
		{
			if (!string.IsNullOrWhiteSpace(pageId))
			{
				var cookieValue = CookieManager.GetPersonalizationCookie(PersonalizationConditions.Visited_Page);

				var regex = new Regex($"{pageId}:\\d*");
				var match = regex.Match(cookieValue);

				if (match.Success)
				{
					var countString = match.Value.Split(":")[1];

					return int.Parse(countString);
				}
			}

			return 0;
		}
	}
}