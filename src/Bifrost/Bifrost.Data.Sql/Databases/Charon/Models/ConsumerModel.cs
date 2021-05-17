using Dapper.Contrib.Extensions;
using Bifrost.Data.Sql.Databases.Bifrost.Objects.Tables;

namespace Bifrost.Data.Sql.Databases.Bifrost.Models
{
    [Table("Consumers")]
    public class ConsumerModel
    {
        [ExplicitKey]
        public string Name { get; set; }
        public string ApiKey { get; set; }

        public bool IsAdmin { get; set; }

        public bool CanEdit { get; set; }

        public string JobsCsv { get; set; }
    }
}
