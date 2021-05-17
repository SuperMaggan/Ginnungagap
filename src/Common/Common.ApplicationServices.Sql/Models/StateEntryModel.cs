using Dapper.Contrib.Extensions;
using Bifrost.Common.ApplicationServices.Sql.Tables;

namespace Bifrost.Common.ApplicationServices.Sql.Models
{
    /// <summary>
    ///     Representing a row in the state table
    /// </summary>
    [Table("StateEntries")]
    public class StateEntryModel
    {
        public int Id { get; set; }
        public string StateName { get; set; }
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
    }
 }