using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Umbraco.Extensions;
using uPersonalize.Enums;
using uPersonalize.Enums.Attributes;
using uPersonalize.Enums.Extensions.Base;

namespace uPersonalize.Tests.Enums
{
	[TestClass]
	public class EnumExtensionsTests : TestClassBase
	{
		[TestMethod]
		public void GetAttribute_Is_Null()
		{
			var attr = PersonalizationConditions.IP_Address.GetAttribute<CookieNameAttribute>();

			Assert.IsNull(attr);
		}

		[TestMethod]
		public void GetAttribute_Is_Not_Null()
		{
			var attr = PersonalizationConditions.Device_Type.GetAttribute<CookieNameAttribute>();

			Assert.IsNotNull(attr);
		}
	}
}