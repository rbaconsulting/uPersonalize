using System.Text.RegularExpressions;

namespace uPersonalize.Constants
{
	public struct Headers
	{
		public struct UserAgent
		{
			public const string Name = "User-Agent";

			public struct RegexRules
			{
				public static readonly Regex Android = new Regex(@"Android");
				public static readonly Regex Windows = new Regex(@"Windows");
			}
		}

		public struct XForwardedFor
		{
			public const string Name = "X-Forwarded-For";

			public struct RegexRules
			{
				public static readonly Regex Ip = new Regex(@"^((\d|(x|X)){3}\.?){4}$");
				public static readonly Regex IpMask = new Regex(@"(x|X){3}");
			}
		}
	}
}