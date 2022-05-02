using uPersonalize.Enums;

namespace uPersonalize.Interfaces
{
	public interface IPersonalizationCookieManager
	{
		string GetPersonalizationCookie(PersonalizationConditions type);
		void SetPersonalizationCookie(PersonalizationConditions type, string cookieValue);
		void SetPairValueListCookie(PersonalizationConditions type, string key, int value = 1);
	}
}