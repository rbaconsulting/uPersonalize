using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;

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
			if (!TableExists(Constants.Migrations.Names.ReportingTableName))
			{
				//TODO
				//Create.Table<uPersonalizeReport>().Do();
			}
			else
			{
				Logger.LogDebug("The database table {DbTable} already exists, skipping", Constants.Migrations.Names.ReportingTableName);
			}
		}
	}
}