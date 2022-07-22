using System.Threading.Tasks;
using uPersonalize.Enums;

namespace uPersonalize.Interfaces
{
	/// <summary>
	/// Used to create and retrieves uPersonalize http request/response cookies. Should not be used explicitly, use <see cref="IPersonalizationService">IPersonalizationService</see> instead.
	/// </summary>
	public interface IPersonalizationCookieManager
	{
		/// <summary>
		/// Finds the matching uPersonalize cookie.
		/// </summary>
		/// <param name="type"></param>
		/// <returns>Cookie value as a string, "string.empty" if no cookie is found.</returns>
		Task<string> GetCookie(PersonalizationConditions type);

		/// <summary>
		/// Creates or overwrites a basic uPersonalize cookie.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="cookieValue"></param>
		/// <returns>true on success, false on failure.</returns>
		Task<bool> SetCookie(PersonalizationConditions type, string cookieValue);

		/// <summary>
		/// Creates or overwrites a list based uPersonalize cookie. Used for cases such as events and page loads.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns>true on success, false on failure.</returns>
		Task<bool> SetKeyValueListCookie(PersonalizationConditions type, string key, int value = 1);

		/// <summary>
		/// Returns if the current user has opted out or not.
		/// </summary>
		/// <returns>true on if a user has opted out, false if not.</returns>
		Task<bool> IsOptOut();

		/// <summary>
		/// Sets the cookie related to opting out a user.
		/// </summary>
		/// <returns>true on success, false on failure.</returns>
		Task<bool> SetOptOut();

		/// <summary>
		/// Deletes all uPersonalize related cookies, excludes opt out cookie by default.
		/// </summary>
		/// <param name="includeOptOut"></param>
		/// <returns>true on success, false on failure.</returns>
		Task<bool> DeleteCookies(bool includeOptOut = false);
	}
}