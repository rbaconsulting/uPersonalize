using System.Text.RegularExpressions;

namespace uPersonalize.Constants
{
	public struct Cookies
	{
		public struct OptOut
		{
			public const string Name = "uPersonalize_opt_out";
		}

		public struct Device
		{
			public const string Name = "uPersonalize_device";
		}

		public struct VisitedPages
		{
			public const string Name = "uPersonalize_visited_pages";

			public struct RegexRules
			{
				public static readonly Regex PageItemId = new("^[1-9]+\\d*$");
			}
		}

		public struct ClickedEvents
		{
			public const string Name = "uPersonalize_clicked_events";

			public struct RegexRules
			{
				public static readonly Regex EventName = new("^[a-zA-Z0-9_-]*$");
			}
		}

		public struct RegexRules
		{
			public static readonly Regex KeyValueListSingle = new("^[[a-zA-Z0-9]+:-?\\d+$");
			public static readonly Regex KeyValueList = new("^([a-zA-Z0-9]+:-?\\d+,)*[[a-zA-Z0-9]+:-?\\d+$");
		}
	}
}