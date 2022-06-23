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
		string Domain { get; set; }

		/// <summary>
		/// Determines if uPersonalize http cookies should be marked as "secure".
		/// </summary>
		bool Secure { get; set; }

		/// <summary>
		/// Determines what <see cref="SameSiteMode">SameSiteMode</see> setting uPersonalize http cookies should use.
		/// </summary>
		SameSiteMode SameSite { get; set; }

		/// <summary>
		/// Determines when uPersonalize http cookies should expire.
		/// </summary>
		TimeSpan MaxAge { get; set; }

		/// <summary>
		/// Retrieves http cookie options from the cookie security related options.
		/// </summary>
		/// <returns> <see cref="Microsoft.AspNetCore.Http.CookieOptions">Microsoft.AspNetCore.Http.CookieOptions</see></returns>
		CookieOptions GetCookieOptions();

		/// <summary>
		/// Saves the model state to uPersonalize-settings.json
		/// </summary>
		Task Save();
	}
}