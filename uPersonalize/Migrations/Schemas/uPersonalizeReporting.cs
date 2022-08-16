using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using uPersonalize.Constants;

namespace uPersonalize.Migrations.Schemas
{
    [TableName(Plugin.Migrations.Reporting.TableName)]
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExplicitColumns]
    public class uPersonalizeReporting
    {
        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Key")]
        public string Key { get; set; }

        [Column("Value")]
        public string Value { get; set; }
    }
}