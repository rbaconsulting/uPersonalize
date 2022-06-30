using Microsoft.Extensions.DependencyInjection;
using uPersonalize.Interfaces;
using uPersonalize.Models;
using uPersonalize.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace uPersonalize.Composers
{
	public class uPersonalizeComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton(PersonalizationSettings.Load());

            builder.Services.AddScoped<IPersonalizationCookieManager, PersonalizationCookieManager>();
            builder.Services.AddScoped<IPersonalizationService, PersonalizationService>();
        }
    }
}