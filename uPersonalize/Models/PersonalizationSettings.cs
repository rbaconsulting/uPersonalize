using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using uPersonalize.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace uPersonalize.Models
{
	public class PersonalizationSettings : IPersonalizationSettings
	{
		private static readonly string _filePath = $"{Environment.CurrentDirectory}/App_Plugins/uPersonalize/uPersonalize-settings.json";

		[JsonProperty]
		public string Domain { get; set; }

		[JsonProperty]
		public bool Secure { get; set; }

		[JsonProperty]
		[JsonConverter(typeof(StringEnumConverter))]
		public SameSiteMode SameSite { get; set; }

		[JsonProperty]
		public TimeSpan MaxAge { get; set; }

		public CookieOptions GetCookieOptions()
		{
			return new CookieOptions()
			{
				Domain = Domain,
				Secure = Secure,
				SameSite = SameSite,
				MaxAge = MaxAge
			};
		}

		public static PersonalizationSettings Load()
        {
			var lines = File.ReadAllText(_filePath);

			return JsonConvert.DeserializeObject<PersonalizationSettings>(lines);
		}

		public async Task Save()
		{
			var fileData = JsonConvert.SerializeObject(this);
			
			await File.WriteAllTextAsync(_filePath, fileData);
		}
	}
}