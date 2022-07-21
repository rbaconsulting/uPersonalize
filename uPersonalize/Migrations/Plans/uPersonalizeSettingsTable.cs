using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;
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

			// Lots of methods available in the MigrationBase class - discover with this.
			if (!TableExists(Constants.Migrations.Names.SettingsTableName))
			{
				Create.Table<uPersonalizeSetting>().Do();
			}
			else
			{
				Logger.LogDebug("The database table {DbTable} already exists, skipping", Constants.Migrations.Names.SettingsTableName);
			}
		}
	}
}