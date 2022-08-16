using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;
using uPersonalize.Constants;
using uPersonalize.Migrations.Schemas;

namespace uPersonalize.Migrations.Plans
{
	public class uPersonalizeSettingsTable : MigrationBase
	{
		public uPersonalizeSettingsTable(IMigrationContext context) : base(context)
		{
		}

		protected override void Migrate()
		{
			Logger.LogDebug("Running migration {MigrationStep}", "AddSettingsTable");

			if (!TableExists(Plugin.Migrations.Settings.TableName))
			{
				Create.Table<uPersonalizeSetting>().Do();
				Logger.LogInformation("{DbTable} database table has been created!", Plugin.Migrations.Settings.TableName);
			}
			else
			{
				Logger.LogDebug("The database table {DbTable} already exists, skipping", Plugin.Migrations.Settings.TableName);
			}
		}
	}
}