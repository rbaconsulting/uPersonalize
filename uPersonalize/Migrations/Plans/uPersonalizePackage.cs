using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Xml;
using System.Xml.Linq;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Packaging;

namespace uPersonalize.Migrations.Plans
{
	public class uPersonalizePackage : PackageMigrationBase
	{
		private readonly ILogger<uPersonalizePackage> _logger;
		private readonly IPackagingService _packagingService;

		public uPersonalizePackage(
			ILogger<uPersonalizePackage> logger,
			IPackagingService packagingService,
			IMediaService mediaService,
			MediaFileManager mediaFileManager,
			MediaUrlGeneratorCollection mediaUrlGenerators,
			IShortStringHelper shortStringHelper,
			IContentTypeBaseServiceProvider contentTypeBaseServiceProvider,
			IMigrationContext context,
			IOptions<PackageMigrationSettings> packageMigrationsSettings)
			: base(packagingService, mediaService, mediaFileManager, mediaUrlGenerators, shortStringHelper, contentTypeBaseServiceProvider, context, packageMigrationsSettings)
		{
			_logger = logger;
			_packagingService = packagingService;
		}

		protected override void Migrate()
		{
			try
			{
				var reader = XmlReader.Create("./App_Plugins/uPersonalize/package.xml");
				var packageDocument = XDocument.Load(reader);

				ImportPackage.FromXmlDataManifest(packageDocument).Do();
				_packagingService.InstallCompiledPackageData(packageDocument);
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
			}
		}
	}
}