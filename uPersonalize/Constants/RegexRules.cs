using System.Text.RegularExpressions;

namespace uPersonalize.Constants
{
	public struct RegexRules
	{
		public struct Conditions
        {
			public const string IP = "^((\\d|(x|X)){3}\\.?){4}$";
			public const string IPMask = @"(x|X){3}";
		}
	}
}