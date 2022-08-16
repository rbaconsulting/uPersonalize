using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using uPersonalize.Migrations.Plans;
using System.Reflection;
using uPersonalize.Constants;
using Microsoft.Extensions.Logging;
using System;

namespace uPersonalize.Migrations
{
	public class uPersonalizeDatabaseMigration : INotificationHandler<UmbracoApplicationStartingNotification>
	{
		private readonly ILogger<uPersonalizeDatabaseMigration> _logger;
		private readonly IScopeProvider _scopeProvider;
		private readonly IMigrationPlanExecutor _migrationPlanExecutor;
		private readonly IKeyValueService _keyValueService;
		private readonly IRuntimeState _runtimeState;

		public uPersonalizeDatabaseMigration(ILogger<uPersonalizeDatabaseMigration> logger,
			IScopeProvider scopeProvider,
			IMigrationPlanExecutor migrationPlanExecutor,
			IKeyValueService keyValueService,
			IRuntimeState runtimeState)
		{
			_logger = logger;
			_scopeProvider = scopeProvider;
			_migrationPlanExecutor = migrationPlanExecutor;
			_keyValueService = keyValueService;
			_runtimeState = runtimeState;
		}

		public void Handle(UmbracoApplicationStartingNotification notification)
		{
			if (_runtimeState.Level < RuntimeLevel.Run)
			{
				return;
			}

			try
			{
				var currentVersion = _keyValueService.GetValue($"Umbraco.Core.Upgrader.State+{Plugin.Migrations.Settings.MigrationKey}") ?? string.Empty;
				var newVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

				var settingsPlan = new MigrationPlan(Plugin.Migrations.Settings.MigrationKey);
				var reportingPlan = new MigrationPlan(Plugin.Migrations.Reporting.MigrationKey);

				if (!newVersion.Equals(currentVersion))
				{
					settingsPlan.From(currentVersion).To<uPersonalizeSettingsTable>(newVersion);
					reportingPlan.From(currentVersion).To<uPersonalizeReportingTable>(newVersion);
				}

				var upgrader = new Upgrader(settingsPlan);
				upgrader.Execute(_migrationPlanExecutor, _scopeProvider, _keyValueService);

				upgrader = new Upgrader(reportingPlan);
				upgrader.Execute(_migrationPlanExecutor, _scopeProvider, _keyValueService);
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
			}
		}
	}
}