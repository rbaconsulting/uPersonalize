using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using uPersonalize.Interfaces;

namespace uPersonalize.Models.Requests
{
	public class SavePersonalizationSettings : IPersonalizationSettings
	{
		public string DomainCookieOption { get; set; }
		public bool SecureCookieOption { get; set; }
		public SameSiteMode SameSiteCookieOption { get; set; }
		public TimeSpan MaxAgeCookieOption { get; set; }

		public CookieOptions GetCookieOptions()
		{
			throw new NotImplementedException();
		}

		public void Load()
		{
			throw new NotImplementedException();
		}

		public Task Save()
		{
			throw new NotImplementedException();
		}
	}
}