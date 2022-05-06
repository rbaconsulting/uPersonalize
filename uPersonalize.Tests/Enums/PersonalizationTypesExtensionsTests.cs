using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using uPersonalize.Enums;
using uPersonalize.Enums.Extensions;

namespace uPersonalize.Tests.Enums
{
	[TestClass]
	public class PersonalizationTypesExtensionsTests : TestClassBase
	{
		[TestMethod]
		public void GetCookieName_Is_Not_Null()
		{
			foreach(PersonalizationConditions personalizationCondition in Enum.GetValues(typeof(PersonalizationConditions)))
			{
				var cookieName = personalizationCondition.GetCookieName();

				Assert.IsNotNull(cookieName);
			}
		}

		[DataRow(PersonalizationConditions.Device_Type)]
		[DataRow(PersonalizationConditions.Visited_Page)]
		[DataRow(PersonalizationConditions.Visited_Page_Count)]
		[DataRow(PersonalizationConditions.Event_Triggered)]
		[DataRow(PersonalizationConditions.Event_Triggered_Count)]
		[TestMethod]
		public void GetCookieName_Valid_Cookies(PersonalizationConditions personalizationCondition)
		{
			Assert.IsTrue(!string.IsNullOrWhiteSpace(personalizationCondition.GetCookieName()));
		}

		[DataRow(PersonalizationConditions.Default)]
		[DataRow(PersonalizationConditions.IP_Address)]
		[TestMethod]
		public void GetCookieName_Invalid_Cookies(PersonalizationConditions personalizationCondition)
		{
			Assert.IsTrue(personalizationCondition.GetCookieName().Equals(string.Empty));
		}
	}
}