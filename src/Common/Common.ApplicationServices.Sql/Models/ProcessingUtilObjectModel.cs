using Dapper.Contrib.Extensions;

namespace Bifrost.Common.ApplicationServices.Sql.Models
{
    [Table("ProcessingUtilObjects")]
    public class ProcessingUtilObjectModel
    {
        [ExplicitKey]
        public string Key { get; set; }
        public string ObjectType { get; set; }
        public string Value { get; set; }
    }
}