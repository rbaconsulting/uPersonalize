using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace uPersonalize.Interfaces
{
	public interface IPersonalizationSettings
	{
		string Domain { get; set; }
		bool Secure { get; set; }
		SameSiteMode SameSite { get; set; }
		TimeSpan MaxAge { get; set; }

		CookieOptions GetCookieOptions();
		Task Save();
	}
}