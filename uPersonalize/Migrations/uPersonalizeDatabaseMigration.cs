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

namespace uPersonalize.Migrations
{
	public class uPersonalizeDatabaseMigration : INotificationHandler<UmbracoApplicationStartingNotification>
	{
		private readonly string _uPersonalizeSettingsKey = "uPersonalize_Settings";
		private readonly string _uPersonalizeReportingKey = "uPersonalize_Reporting";

		private readonly IScopeProvider _scopeProvider;
		private readonly IMigrationPlanExecutor _migrationPlanExecutor;
		private readonly IKeyValueService _keyValueService;
		private readonly IRuntimeState _runtimeState;

		public uPersonalizeDatabaseMigration(
			IScopeProvider scopeProvider,
			IMigrationPlanExecutor migrationPlanExecutor,
			IKeyValueService keyValueService,
			IRuntimeState runtimeState)
		{
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

			var currentVersion = _keyValueService.GetValue($"Umbraco.Core.Upgrader.State+{_uPersonalizeSettingsKey}");
			var newVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

			var settingsPlan = new MigrationPlan(_uPersonalizeSettingsKey);
			var reportingPlan = new MigrationPlan(_uPersonalizeReportingKey);

			if(!newVersion.Equals(currentVersion) && !newVersion.Equals(currentVersion))
			{
				settingsPlan.From(currentVersion).To<uPersonalizeSettingsTable>(newVersion);
				reportingPlan.From(currentVersion).To<uPersonalizeReportingTable>(newVersion);
			}

			settingsPlan.From(string.Empty).To<uPersonalizeSettingsTable>(newVersion);
			reportingPlan.From(string.Empty).To<uPersonalizeReportingTable>(newVersion);

			ExecutePlan(settingsPlan);
			ExecutePlan(reportingPlan);
		}

		private void ExecutePlan(MigrationPlan plan)
		{
			var upgrader = new Upgrader(plan);
			upgrader.Execute(_migrationPlanExecutor, _scopeProvider, _keyValueService);
		}
	}
}