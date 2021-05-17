using System;
using Dapper.Contrib.Extensions;

namespace Bifrost.Common.ApplicationServices.Sql.Models
{
    [Table("Jobs")]
    public class JobModel
    {
        [ExplicitKey]
        public string Name { get; set; }
        public string Description { get; set; }
        public string JobType { get; set; }
        public string TriggerCronSyntax { get; set; }
        public int ConcurrentLimit { get; set; }
        public bool Enabled { get; set; }
        public string Configuration { get; set; }
        public DateTime LastUpdated { get; set; }
    }

}