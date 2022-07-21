using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace uPersonalize.Migrations.Schemas
{
	[TableName(Constants.Migrations.Names.SettingsTableName)]
    [PrimaryKey("Key", AutoIncrement = false)]
    [ExplicitColumns]
    public class uPersonalizeSetting
    {
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column("Key")]
        public string Key { get; set; }

        [Column("Value")]
        public string Value { get; set; }
    }
}