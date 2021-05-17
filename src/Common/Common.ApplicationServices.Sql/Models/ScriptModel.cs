using System;
using Dapper.Contrib.Extensions;

namespace Bifrost.Common.ApplicationServices.Sql.Models
{
    [Table("Scripts")]
    public class ScriptModel
    {
        [ExplicitKey]
        public string Name { get; set; }
        public DateTime LastUpdated { get; set; }
        public string DomainName { get; set; }
        public string Code { get; set; }
        public string ImportedScriptNamesCsv { get; set; }
        public string Owner { get; set; }
        public bool IsPublic { get; set; }
        public string Metadata { get; set; }
    }

}
