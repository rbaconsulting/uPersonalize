using uPersonalize.Enums.Attributes;
using uPersonalize.Enums.Extensions.Base;

namespace uPersonalize.Enums.Extensions
{
	public static class PersonalizationTypesExtensions
	{
        public static string GetCookieName(this PersonalizationConditions role)
        {
            var attr = role.GetAttribute<CookieNameAttribute>();

			return attr != null ? attr.Value : string.Empty;
		}
	}
}