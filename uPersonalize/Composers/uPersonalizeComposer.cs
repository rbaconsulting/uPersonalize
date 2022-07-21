using Microsoft.Extensions.DependencyInjection;
using uPersonalize.Interfaces;
using uPersonalize.Models;
using uPersonalize.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using uPersonalize.Migrations;

namespace uPersonalize.Composers
{
	public class uPersonalizeComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<UmbracoApplicationStartingNotification, uPersonalizeDatabaseMigration>();

            builder.Services.AddSingleton<IPersonalizationSettings, PersonalizationSettings>();
            builder.Services.AddScoped<IPersonalizationCookieManager, PersonalizationCookieManager>();
            builder.Services.AddScoped<IPersonalizationService, PersonalizationService>();
        }
    }
}