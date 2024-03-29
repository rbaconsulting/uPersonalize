﻿using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Web;
using uPersonalize.Enums;
using uPersonalize.Enums.Extensions;
using System.Linq;
using uPersonalize.Services;

namespace uPersonalize.Tests
{
	public class TestClassBase
	{
		public HttpContext HttpContext { get; set; }

		public void SetupBase()
		{
			HttpContext = new DefaultHttpContext();

			var cookies = $"{PersonalizationConditions.Device_Type.GetCookieName()}=Windows;{PersonalizationConditions.Visited_Page.GetCookieName()}=10:1;{PersonalizationConditions.Event_Triggered.GetCookieName()}=testEvent:1;";
			var requestFeature = new HttpRequestFeature
			{
				Headers = new HeaderDictionary
				{
					{
						HeaderNames.Cookie,
						new StringValues(cookies)
					}
				}
			};

			var featureCollection = new FeatureCollection();
			featureCollection.Set<IHttpRequestFeature>(requestFeature);

			var cookiesFeature = new RequestCookiesFeature(featureCollection);

			HttpContext.Request.Cookies = cookiesFeature.Cookies;
		}

		protected PersonalizationService InitializePersonalizationService(bool initMemberManager, bool withCookieValues, bool cookiesAsList)
		{
			var cookieManager = MoqProvider.PersonalizationCookieManager(withCookieValues, cookiesAsList);

			return new PersonalizationService(MoqProvider.Logger<PersonalizationService>(),
											  MoqProvider.MemberManager(initMemberManager),
											  MoqProvider.HttpContextAccessor(HttpContext),
											  cookieManager);
		}

		protected string GetUnitTestCookie(PersonalizationConditions personalizationCondition)
		{
			var cookieName = personalizationCondition.GetCookieName();
			var cookieValue = HttpContext.Response.Headers.Values.FirstOrDefault(h => h.Any(v => v.StartsWith(cookieName))).FirstOrDefault();

			return HttpUtility.UrlDecode(cookieValue.Replace($"{cookieName}=", "").Replace($"; path=/", ""));
		}
	}
}