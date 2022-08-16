using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using uPersonalize.Constants;
using uPersonalize.Enums;
using uPersonalize.Enums.Extensions;
using uPersonalize.Services;

namespace uPersonalize.Tests.Services
{
	[TestClass]
	public class PersonalizationCookieManagerTests : TestClassBase
	{
		private PersonalizationCookieManager _personalizationCookieManager;

		[TestInitialize]
		public void Setup()
		{
			SetupBase();

			_personalizationCookieManager = new PersonalizationCookieManager(MoqProvider.Logger<PersonalizationCookieManager>(), 
																			 MoqProvider.HttpContextAccessor(HttpContext),
																			 MoqProvider.PersonalizationSettings());
		}

		[DataRow(PersonalizationConditions.Device_Type, "Windows")]
		[DataRow(PersonalizationConditions.Visited_Page, "10:1")]
		[DataRow(PersonalizationConditions.Visited_Page_Count, "10:1")]
		[DataRow(PersonalizationConditions.Event_Triggered, "testEvent:1")]
		[DataRow(PersonalizationConditions.Event_Triggered_Count, "testEvent:1")]
		[TestMethod]
		public void SetCookie_Valid_Conditions_Valid_Values(PersonalizationConditions personalizationCondition, string value)
		{
			_personalizationCookieManager.SetCookie(personalizationCondition, value).Wait();

			var cookieName = personalizationCondition.GetCookieName();

			Assert.IsTrue(HttpContext.Response.Headers.Values.Any(h => h.Any(v => v.StartsWith(cookieName))));
		}

		[DataRow(PersonalizationConditions.Device_Type)]
		[DataRow(PersonalizationConditions.Visited_Page)]
		[DataRow(PersonalizationConditions.Visited_Page_Count)]
		[DataRow(PersonalizationConditions.Event_Triggered)]
		[DataRow(PersonalizationConditions.Event_Triggered_Count)]
		[TestMethod]
		public void SetCookie_Valid_Conditions_Invalid_Values(PersonalizationConditions personalizationCondition)
		{
			_personalizationCookieManager.SetCookie(personalizationCondition, null).Wait();
			_personalizationCookieManager.SetCookie(personalizationCondition, string.Empty).Wait();

			Assert.IsTrue(HttpContext.Response.Headers.Values.Count == 0);
		}

		[DataRow(PersonalizationConditions.Default)]
		[DataRow(PersonalizationConditions.IP_Address)]
		[TestMethod]
		public void SetCookie_Invalid_Conditions(PersonalizationConditions personalizationCondition)
		{
			_personalizationCookieManager.SetCookie(personalizationCondition, "dummyText").Wait();
			_personalizationCookieManager.SetCookie(personalizationCondition, string.Empty).Wait();

			Assert.IsTrue(HttpContext.Response.Headers.Values.Count == 0);
		}

		[DataRow(PersonalizationConditions.Device_Type, "Windows")]
		[DataRow(PersonalizationConditions.Visited_Page, "10:1")]
		[DataRow(PersonalizationConditions.Visited_Page_Count, "10:1")]
		[DataRow(PersonalizationConditions.Event_Triggered, "testEvent:1")]
		[DataRow(PersonalizationConditions.Event_Triggered_Count, "testEvent:1")]
		[TestMethod]
		public async Task GetCookie_Valid_Conditions(PersonalizationConditions personalizationCondition, string value)
		{
			Assert.AreEqual(value, await _personalizationCookieManager.GetCookie(personalizationCondition));
		}

		[DataRow(PersonalizationConditions.Default)]
		[DataRow(PersonalizationConditions.IP_Address)]
		[TestMethod]
		public async Task GetCookie_Invalid_Conditions(PersonalizationConditions personalizationCondition)
		{
			Assert.IsTrue(string.IsNullOrWhiteSpace(await _personalizationCookieManager.GetCookie(personalizationCondition)));
		}

		[DataRow(PersonalizationConditions.Visited_Page, "10")]
		[DataRow(PersonalizationConditions.Visited_Page_Count, "10")]
		[DataRow(PersonalizationConditions.Event_Triggered, "testEvent")]
		[DataRow(PersonalizationConditions.Event_Triggered_Count, "testEvent")]
		[TestMethod]
		public void SetKeyValueListCookie_Valid_Conditions(PersonalizationConditions personalizationCondition, string key)
		{
			_personalizationCookieManager.SetKeyValueListCookie(personalizationCondition, key).Wait();

			var cookieValue = GetUnitTestCookie(personalizationCondition);

			Assert.IsTrue(HttpContext.Response.Headers.Values.Count == 1);
			Assert.IsTrue(Cookies.Shared.RegexRules.KeyValueListSingle.IsMatch(cookieValue));
			Assert.AreEqual($"{key}:2", cookieValue);
		}

		[DataRow(PersonalizationConditions.Device_Type)]
		[DataRow(PersonalizationConditions.Default)]
		[DataRow(PersonalizationConditions.IP_Address)]
		[TestMethod]
		public void SetKeyValueListCookie_Invalid_Conditions(PersonalizationConditions personalizationCondition)
		{
			_personalizationCookieManager.SetKeyValueListCookie(personalizationCondition, string.Empty).Wait();
			_personalizationCookieManager.SetKeyValueListCookie(personalizationCondition, string.Empty, 10).Wait();
			_personalizationCookieManager.SetKeyValueListCookie(personalizationCondition, "testEvent").Wait();

			Assert.IsTrue(HttpContext.Response.Headers.Values.Count == 0);
		}

		[TestMethod]
		public void SetKeyValueListCookie_New_Cookie()
		{
			HttpContext.Request.Headers.Clear();

			_personalizationCookieManager.SetKeyValueListCookie(PersonalizationConditions.Visited_Page, "12345").Wait();

			Assert.IsTrue(HttpContext.Response.Headers.Values.Count == 1);
			Assert.IsTrue(Cookies.Shared.RegexRules.KeyValueListSingle.IsMatch(GetUnitTestCookie(PersonalizationConditions.Visited_Page)));
		}

		[TestMethod]
		public void SetKeyValueListCookie_New_Key()
		{
			_personalizationCookieManager.SetKeyValueListCookie(PersonalizationConditions.Visited_Page, "12345").Wait();

			Assert.IsTrue(HttpContext.Response.Headers.Values.Count == 1);

			Assert.IsTrue(Cookies.Shared.RegexRules.KeyValueList.IsMatch(GetUnitTestCookie(PersonalizationConditions.Visited_Page)));
		}

		[TestMethod]
		public void SetKeyValueListCookie_Existing_Key()
		{
			_personalizationCookieManager.SetKeyValueListCookie(PersonalizationConditions.Visited_Page, "10").Wait();

			var cookieValue = GetUnitTestCookie(PersonalizationConditions.Visited_Page);

			Assert.AreEqual("10:2", cookieValue);
			Assert.IsTrue(HttpContext.Response.Headers.Values.Count == 1);

			Assert.IsTrue(Cookies.Shared.RegexRules.KeyValueListSingle.IsMatch(cookieValue));
		}

		[TestMethod]
		public void SetKeyValueListCookie_Invalid_Keys()
		{
			_personalizationCookieManager.SetKeyValueListCookie(PersonalizationConditions.Visited_Page, string.Empty).Wait();
			_personalizationCookieManager.SetKeyValueListCookie(PersonalizationConditions.Visited_Page, null).Wait();

			Assert.IsTrue(HttpContext.Response.Headers.Values.Count == 0);
		}

		[TestMethod]
		public void SetKeyValueListCookie_No_Value()
		{
			_personalizationCookieManager.SetKeyValueListCookie(PersonalizationConditions.Visited_Page, "10").Wait();

			Assert.IsTrue(HttpContext.Response.Headers.Values.Count == 1);
			Assert.AreEqual($"10:2", GetUnitTestCookie(PersonalizationConditions.Visited_Page));
		}

		[TestMethod]
		public void SetKeyValueListCookie_Valid_Values()
		{
			var random = new Random();

			for (var i = 0; i < 10; i++)
			{
				var value = random.Next();
				_personalizationCookieManager.SetKeyValueListCookie(PersonalizationConditions.Visited_Page, "10", value).Wait();

				Assert.IsTrue(HttpContext.Response.Headers.Values.Count == 1);
				Assert.AreEqual($"10:{1 + value}", GetUnitTestCookie(PersonalizationConditions.Visited_Page));

				HttpContext.Response.Headers.Clear();
			}
		}
	}
}