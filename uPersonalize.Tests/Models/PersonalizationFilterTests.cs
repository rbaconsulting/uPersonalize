using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System;
using uPersonalize.Models;

namespace uPersonalize.Tests.Models
{
	[TestClass]
	public class PersonalizationFilterTests : TestClassBase
	{
		private readonly string goodJsonData = "{\"condition\":\"IP_Address\",\"action\":\"Hide\",\"ipAddress\":\"174.141.202.198\",\"deviceToMatch\":0,\"pageId\":\"1086\",\"eventName\":\"\",\"pageEventCount\":0,\"dateTimeCompare\":\"2022-07-29T05:00:00.000\",\"additionalClasses\":\"\"}";

		private readonly string badJsonData = "{\"condition\": null,\"action\": null,\"ipAddress\":\"\",\"deviceToMatch\": null,\"pageId\":\"1086\",\"eventName\":\"\",\"pageEventCount\": null,\"dateTimeCompare\":\"\",\"additionalClasses\": null}";


		[TestMethod]
		public void Empty_Filter()
		{
			var filter = new PersonalizationFilter();

			Assert.IsTrue(!filter.IsValid());
		}

		[TestMethod]
		public void Create_Valid_Json()
		{
			var filter = PersonalizationFilter.Create(goodJsonData);

			Assert.IsTrue(filter.IsValid());
		}

		[TestMethod]
		public void Create_Invalid_Json()
		{
			var filter = PersonalizationFilter.Create(badJsonData);

			Assert.IsTrue(!filter.IsValid());
		}

		[TestMethod]
		public void Create_Empty_Json()
		{
			var filter = PersonalizationFilter.Create(string.Empty);

			Assert.IsTrue(!filter.IsValid());
		}
	}
}