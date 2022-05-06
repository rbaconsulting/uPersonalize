using System.Threading.Tasks;
using uPersonalize.Enums;

namespace uPersonalize.Interfaces
{
	/// <summary>
	/// A service that sets and gets uPersonalize related cookies. Use the IPersonalizationService before trying to use this service directly.
	/// </summary>
	public interface IPersonalizationCookieManager
	{
		/// <summary>
		/// Finds the uPersonalize related cookie by passed PersonalizationConditions enum type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns>Cookie value as a string, "string.empty" if no cookie is found.</returns>
		Task<string> GetCookie(PersonalizationConditions type);

		/// <summary>
		/// Tries to set a uPersonalize related cookie with a designated value. Cookie name is based off of PersonalizationConditions enum type.
		/// Original cookie value will be overwritten if found.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="cookieValue"></param>
		/// <returns>true on success, false on failure.</returns>
		Task<bool> TrySetCookie(PersonalizationConditions type, string cookieValue);

		/// <summary>
		/// Tries to set
		/// </summary>
		/// <param name="type"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns>true on success, false on failure.</returns>
		Task<bool> TrySetKeyValueListCookie(PersonalizationConditions type, string key, int value = 1);
	}
}