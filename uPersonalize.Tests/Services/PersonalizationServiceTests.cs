using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Threading.Tasks;
using uPersonalize.Enums;
using uPersonalize.Models;
using uPersonalize.Services;

namespace uPersonalize.Tests.Services
{
	[TestClass]
	public class PersonalizationServiceTests : TestClassBase
	{
		private PersonalizationService _personalizationService;
		protected Random Random { get; set; }

		[TestInitialize]
		public void Setup()
		{
			SetupBase();

			_personalizationService = new PersonalizationService(MoqProvider.Logger<PersonalizationService>(),
																 MoqProvider.MemberManager(),
																 MoqProvider.HttpContextAccessor(HttpContext),
																 MoqProvider.PersonalizationCookieManager(true, false));
			Random = new Random();
		}

		[TestMethod]
		public async Task TrackUser_Valid_PageId()
		{
			await _personalizationService.TrackUser(Random.Next(1, 300000));
		}

		[DataRow(-1)]
		[DataRow(0)]
		[TestMethod]
		public async Task TrackUser_Invalid_PageId(int pageId)
		{
			await _personalizationService.TrackUser(pageId);
		}

		[TestMethod]
		public async Task IsMatch_Valid_Filter_Match()
		{
			HttpContext.Connection.RemoteIpAddress = IPAddress.None;

			foreach (PersonalizationConditions personalizationCondition in Enum.GetValues(typeof(PersonalizationConditions)))
			{
				if(personalizationCondition == PersonalizationConditions.Default)
				{
					continue;
				}

				var filter = new PersonalizationFilter()
				{
					IpAddress = IPAddress.None.ToString(),
					Condition = personalizationCondition,
					DeviceToMatch = DeviceTypes.Windows,
					PageId = "10",
					EventName = "testEvent",
					PageEventCount = 1
				};

				Assert.IsTrue(await _personalizationService.IsMatch(filter));
			}
		}

		[DataRow(PersonalizationConditions.Visited_Page)]
		[DataRow(PersonalizationConditions.Visited_Page_Count)]
		[DataRow(PersonalizationConditions.Event_Triggered)]
		[DataRow(PersonalizationConditions.Event_Triggered_Count)]
		[TestMethod]
		public async Task IsMatch_Valid_Filter_Key_Value_List_Match(PersonalizationConditions personalizationCondition)
		{
			_personalizationService = new PersonalizationService(MoqProvider.Logger<PersonalizationService>(),
																 MoqProvider.MemberManager(),
																 MoqProvider.HttpContextAccessor(HttpContext),
																 MoqProvider.PersonalizationCookieManager(false, true));

			var filter = new PersonalizationFilter()
			{
				Condition = personalizationCondition,
				PageId = "10",
				EventName = "testEvent",
				PageEventCount = 1
			};

			Assert.IsTrue(await _personalizationService.IsMatch(filter));
		}

		[TestMethod]
		public async Task IsMatch_Valid_Filter_No_Match()
		{
			foreach (PersonalizationConditions personalizationCondition in Enum.GetValues(typeof(PersonalizationConditions)))
			{
				var filter = new PersonalizationFilter()
				{
					IpAddress = "127.1.1.1",
					DeviceToMatch = DeviceTypes.Android,
					Condition = personalizationCondition,
					EventName = "notFound",
					PageId = "15",
					PageEventCount = 2
				};

				Assert.IsFalse(await _personalizationService.IsMatch(filter));
			}
		}

		[DataRow(PersonalizationConditions.Visited_Page)]
		[DataRow(PersonalizationConditions.Visited_Page_Count)]
		[DataRow(PersonalizationConditions.Event_Triggered)]
		[DataRow(PersonalizationConditions.Event_Triggered_Count)]
		[TestMethod]
		public async Task IsMatch_Valid_Filter_Key_Value_List_No_Match(PersonalizationConditions personalizationCondition)
		{
			_personalizationService = new PersonalizationService(MoqProvider.Logger<PersonalizationService>(),
																 MoqProvider.MemberManager(),
																 MoqProvider.HttpContextAccessor(HttpContext),
																 MoqProvider.PersonalizationCookieManager(false, true));

			var filter = new PersonalizationFilter()
			{
				Condition = personalizationCondition,
				EventName = "notFound",
				PageId = "15",
				PageEventCount = 2
			};

			Assert.IsFalse(await _personalizationService.IsMatch(filter));
		}

		[TestMethod]
		public async Task IsMatch_Invalid_Filter()
		{
			foreach (PersonalizationConditions personalizationCondition in Enum.GetValues(typeof(PersonalizationConditions)))
			{
				var filter = new PersonalizationFilter()
				{
					IpAddress = IPAddress.Loopback.ToString(),
					DeviceToMatch = DeviceTypes.Default,
					Condition = personalizationCondition,
					EventName = "!@#$",
					PageId = "!@#$",
					PageEventCount = -1
				};

				Assert.IsFalse(await _personalizationService.IsMatch(filter));
			}
		}

		[TestMethod]
		public async Task IsMatch_Empty_Cookies()
		{
			_personalizationService = new PersonalizationService(MoqProvider.Logger<PersonalizationService>(),
																 MoqProvider.MemberManager(),
																 MoqProvider.HttpContextAccessor(HttpContext),
																 MoqProvider.PersonalizationCookieManager(false, false));

			foreach (PersonalizationConditions personalizationCondition in Enum.GetValues(typeof(PersonalizationConditions)))
			{
				if (personalizationCondition == PersonalizationConditions.Default || personalizationCondition == PersonalizationConditions.IP_Address)
				{
					continue;
				}

				var filter = new PersonalizationFilter()
				{
					Condition = personalizationCondition,
					DeviceToMatch = DeviceTypes.Windows,
					PageId = "10",
					EventName = "testEvent",
					PageEventCount = 1
				};

				Assert.IsFalse(await _personalizationService.IsMatch(filter));
			}
		}

		[TestMethod]
		public async Task IsMatch_Empty_Filter()
		{
			var filter = new PersonalizationFilter();

			Assert.IsFalse(await _personalizationService.IsMatch(filter));
		}

		[TestMethod]
		public async Task TryPageVisit_Valid_PageId()
		{
			Assert.IsTrue(await _personalizationService.TryPageVisit(Random.Next(1, 300000).ToString()));
		}

		[DataRow("0")]
		[DataRow("-1")]
		[DataRow("")]
		[DataRow("asdf")]
		[DataRow("asdf!~")]
		[DataRow(null)]
		[TestMethod]
		public async Task TryPageVisit_Invalid_PageId(string pageId)
		{
			Assert.IsFalse(await _personalizationService.TryPageVisit(pageId));
		}

		[DataRow("testEvent")]
		[DataRow("test_event")]
		[DataRow("test-event")]
		[DataRow("testEvent1")]
		[TestMethod]
		public async Task TryTriggerEvent_Valid_EventId(string eventId)
		{
			Assert.IsTrue(await _personalizationService.TryTriggerEvent(eventId));
		}

		[DataRow(null)]
		[DataRow("")]
		[DataRow("test event")]
		[DataRow("testEvent!")]
		[DataRow("test~event")]
		[DataRow("test.event")]
		[DataRow("!@#@$%")]
		[TestMethod]
		public async Task TryTriggerEvent_Invalid_EventId(string eventId)
		{
			Assert.IsFalse(await _personalizationService.TryTriggerEvent(eventId));
		}

		[TestMethod]
		public async Task GetPageVisitCount_Valid_PageId()
		{
			Assert.AreEqual(1, await _personalizationService.GetPageVisitCount("10"));
		}

		[DataRow("0")]
		[DataRow("-1")]
		[DataRow("")]
		[DataRow("asdf")]
		[DataRow("asdf!~")]
		[DataRow(null)]
		[TestMethod]
		public async Task GetPageVisitCount_Invalid_PageId(string pageId)
		{
			Assert.AreEqual(-1, await _personalizationService.GetPageVisitCount(pageId));
		}

		[TestMethod]
		public async Task GetTriggeredEventCount_Valid_EventId()
		{
			Assert.AreEqual(1, await _personalizationService.GetTriggeredEventCount("testEvent"));
		}

		[DataRow(null)]
		[DataRow("")]
		[DataRow("test event")]
		[DataRow("testEvent!")]
		[DataRow("test~event")]
		[DataRow("test.event")]
		[DataRow("!@#@$%")]
		[TestMethod]
		public async Task GetTriggeredEventCount_Invalid_EventId(string eventId)
		{
			Assert.AreEqual(-1, await _personalizationService.GetTriggeredEventCount(eventId));
		}
	}
}