using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using uPersonalize.Constants;

namespace uPersonalize.Migrations.Schemas
{
	[TableName(Plugin.Migrations.Reporting.TableName)]
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