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
				public static readonly Regex iPhone = new Regex(@"iPhone|iPod|iPad");
				public static readonly Regex Windows = new Regex(@"Windows");
				public static readonly Regex Linux = new Regex(@"Linux");
				public static readonly Regex Mac = new Regex(@"Macintosh");
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