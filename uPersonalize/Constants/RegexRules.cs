using System.Text.RegularExpressions;

namespace uPersonalize.Constants
{
	public struct RegexRules
	{
		public struct Umbraco
		{
			public const string PageItemId = @"^[1-9]+\d*$";
		}

		public struct Events
		{
			public const string Name = @"^[a-zA-Z0-9_-]*$";
		}

		public struct Cookies
		{
			public struct Values
			{
				public const string KeyValueListSingle = @"^[[a-zA-Z0-9]+:-?\d+$";
				public const string KeyValueList = @"^([a-zA-Z0-9]+:-?\d+,)*[[a-zA-Z0-9]+:-?\d+$";
			}
		}

		public struct Conditions
        {
			public const string IP = "^((\\d|(x|X)){3}\\.?){4}$";
			public const string IPMask = @"(x|X){3}";
		}

		public struct UserAgents
        {
			public const string Android = @"Android";
			public const string Windows = @"Windows";
		}
	}
}