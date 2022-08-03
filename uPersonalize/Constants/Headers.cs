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
	}
}