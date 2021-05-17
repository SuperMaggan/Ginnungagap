using System;
using Dapper.Contrib.Extensions;

namespace Bifrost.Common.ApplicationServices.Sql.Models
{
    [Table("ProcessInformation")]
    public class ProcessInformationModel
    {
        /// <summary>
        ///     Usually the file path to the process' exe
        /// </summary>
        [ExplicitKey]
        public string Id { get; set; }

        public string ProcessType { get; set; }
        public string ServerName { get; set; }
        public DateTime LastUpdated { get; set; }
        public string InformationCsv { get; set; }

        /// <summary>
        ///     How frequent this process information gets updated
        /// </summary>
        public int UpdateFrequencySec { get; set; }
    }
}