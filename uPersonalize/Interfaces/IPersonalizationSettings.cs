using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace uPersonalize.Interfaces
{
	/// <summary>
	/// Used to store editable settings within the uPersonalizes dashboard.
	/// </summary>
	public interface IPersonalizationSettings
	{
		/// <summary>
		/// Determines what the domain should be for uPersonalize http cookies.
		/// </summary>
		string DomainCookieOption { get; set; }

		/// <summary>
		/// Determines if uPersonalize http cookies should be marked as "secure".
		/// </summary>
		bool SecureCookieOption { get; set; }

		/// <summary>
		/// Determines what <see cref="SameSiteMode">SameSiteMode</see> setting uPersonalize http cookies should use.
		/// </summary>
		SameSiteMode SameSiteCookieOption { get; set; }

		/// <summary>
		/// Determines when uPersonalize http cookies should expire.
		/// </summary>
		TimeSpan MaxAgeCookieOption { get; set; }

		/// <summary>
		/// Retrieves http cookie options from the cookie security related options.
		/// </summary>
		/// <returns> <see cref="CookieOptions">Microsoft.AspNetCore.Http.CookieOptions</see></returns>
		CookieOptions GetCookieOptions();

		/// <summary>
		/// Loads the model state from the settings database table
		/// </summary>
		void Load();

		/// <summary>
		/// Saves the model state to the settings database table
		/// </summary>
		Task Save();
	}
}