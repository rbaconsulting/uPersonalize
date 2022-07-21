using Microsoft.AspNetCore.Http;
using uPersonalize.Interfaces;
using System;
using System.Threading.Tasks;
using uPersonalize.Migrations.Schemas;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Scoping;

namespace uPersonalize.Models
{
	public class PersonalizationSettings : IPersonalizationSettings
	{
		private readonly ILogger<PersonalizationSettings> _logger;
		private readonly IScopeProvider _scopeProvider;
		private readonly string _settingsQueryAll = "select * from dbo.uPersonalizeSettings";
		private readonly string _settingsQueryByKey = "select * from dbo.uPersonalizeSettings where [Key] = '{0}'";

		public string DomainCookieOption { get; set; }
		public bool SecureCookieOption { get; set; }
		public SameSiteMode SameSiteCookieOption { get; set; }
		public TimeSpan MaxAgeCookieOption { get; set; }

		public PersonalizationSettings(ILogger<PersonalizationSettings> logger, IScopeProvider scopeProvider)
		{
			_logger = logger;
			_scopeProvider = scopeProvider;
			DomainCookieOption = string.Empty;
			SecureCookieOption = true;
			SameSiteCookieOption = SameSiteMode.Strict;
			MaxAgeCookieOption = TimeSpan.FromDays(365);

			Load();
		}

		public CookieOptions GetCookieOptions()
		{
			return new CookieOptions()
			{
				Domain = DomainCookieOption,
				Secure = SecureCookieOption,
				SameSite = SameSiteCookieOption,
				MaxAge = MaxAgeCookieOption
			};
		}

		public void Load()
		{
			try
			{
				using (var scope = _scopeProvider.CreateScope(autoComplete: true))
				{
					var settings = scope.Database.Query<uPersonalizeSetting>(_settingsQueryAll);

					foreach (var setting in settings)
					{
						switch (setting.Key)
						{
							case nameof(DomainCookieOption):
								DomainCookieOption = setting.Value;
								break;
							case nameof(SecureCookieOption):
								SecureCookieOption = bool.Parse(setting.Value);
								break;
							case nameof(SameSiteCookieOption):
								SameSiteCookieOption = Enum.Parse<SameSiteMode>(setting.Value);
								break;
							case nameof(MaxAgeCookieOption):
								MaxAgeCookieOption = TimeSpan.Parse(setting.Value);
								break;
							default:
								break;
						}
					}
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
			}
		}

		public async Task Save()
		{
			try
			{
				using (var scope = _scopeProvider.CreateScope(autoComplete: true))
				{
					foreach (var property in GetType().GetProperties())
					{
						var propertyValue = property.GetValue(this).ToString();

						if (scope.Database.Exists<uPersonalizeSetting>(property.Name))
						{
							var setting = await scope.Database.FirstOrDefaultAsync<uPersonalizeSetting>(string.Format(_settingsQueryByKey, property.Name));

							setting.Value = propertyValue;

							scope.Database.Save(setting);
						}
						else
						{
							var newSetting = new uPersonalizeSetting()
							{
								Key = property.Name,
								Value = propertyValue
							};

							await scope.Database.InsertAsync(newSetting);
						}
					}
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
			}
		}
	}
}