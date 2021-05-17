using Dapper.Contrib.Extensions;

namespace Bifrost.Common.ApplicationServices.Sql.Models
{
    [Table("WorkTasks")]
    public class WorkTaskModel
    {
        public int Id { get; set; }
        public string Owner { get; set; }
        public bool CheckedOut { get; set; }
        public string InstructionsCsv { get; set; }
    }
}