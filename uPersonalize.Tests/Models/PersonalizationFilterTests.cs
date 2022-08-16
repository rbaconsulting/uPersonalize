using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System;
using uPersonalize.Models;

namespace uPersonalize.Tests.Models
{
	[TestClass]
	public class PersonalizationFilterTests : TestClassBase
	{
		private readonly string jsonData = "{\"condition\":\"IP_Address\",\"action\":\"Hide\",\"ipAddress\":\"174.141.202.198\",\"deviceToMatch\":0,\"pageId\":\"1086\",\"eventName\":\"\",\"pageEventCount\":0,\"dateTimeCompare\":\"2022-07-29T05:00:00.000\",\"additionalClasses\":\"\"}";

		[TestMethod]
		public void Empty_Filter()
		{
			var filter = new PersonalizationFilter();

			Assert.IsTrue(!filter.IsValid());
		}

		[TestMethod]
		public void Create_Valid_Json()
		{
			var filter = PersonalizationFilter.Create(jsonData);

			Assert.IsTrue(filter.IsValid());
		}

		[TestMethod]
		public void Create_Invalid_Json()
		{
			var filter = PersonalizationFilter.Create(string.Empty);

			Assert.IsTrue(!filter.IsValid());
		}
	}
}