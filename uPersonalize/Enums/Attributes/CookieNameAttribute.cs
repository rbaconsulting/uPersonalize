using System;

namespace uPersonalize.Enums.Attributes
{
	public class CookieNameAttribute : Attribute
	{
		public string Value { get; }

		public CookieNameAttribute(string value)
		{
			Value = value;
		}
	}
}