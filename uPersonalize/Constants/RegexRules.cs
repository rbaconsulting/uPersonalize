using System.Text.RegularExpressions;

namespace uPersonalize.Constants
{
	public struct RegexRules
	{
		public struct Umbraco
		{
			public static Regex PageItemId => new(@"^[1-9]+\d*$");
		}

		public struct Events
		{
			public static Regex Name => new(@"^[a-zA-Z0-9_-]*$");
		}

		public struct Cookies
		{
			public struct Values
			{
				public static Regex KeyValueListSingle => new(@"^[[a-zA-Z0-9]+:-?\d+$");
				public static Regex KeyValueList => new(@"^([a-zA-Z0-9]+:-?\d+,)*[[a-zA-Z0-9]+:-?\d+$");
			}
		}
	}
}