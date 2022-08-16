using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;
using uPersonalize.Constants;

namespace uPersonalize.Migrations.Plans
{
	public class uPersonalizeReportingTable : MigrationBase
	{
		public uPersonalizeReportingTable(IMigrationContext context) : base(context)
		{
		}

		protected override void Migrate()
		{
			Logger.LogDebug("Running migration {MigrationStep}", "AddReportingTable");

			// Lots of methods available in the MigrationBase class - discover with this.
			if (!TableExists(Plugin.Migrations.Reporting.TableName))
			{
				//TODO
				//Create.Table<uPersonalizeReport>().Do();
				//Logger.LogInformation("{DbTable} database table has been created!", Plugin.Migrations.Reporting.TableName);
			}
			else
			{
				Logger.LogDebug("The database table {DbTable} already exists, skipping", Plugin.Migrations.Reporting.TableName);
			}
		}
	}
}