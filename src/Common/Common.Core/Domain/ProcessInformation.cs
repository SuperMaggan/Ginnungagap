using System;
using System.Collections.Generic;

namespace Bifrost.Common.Core.Domain
{
    public class ProcessInformation
    {
        public ProcessInformation(string id)
        {
            Id = id;
            Information = new List<Field>();
        }

        /// <summary>
        ///     Usually the file path to the process' exe
        /// </summary>
        public string Id { get; }

        public string ProcessType { get; set; }
        public string ServerName { get; set; }
        public DateTime LastUpdated { get; set; }

        /// <summary>
        ///     How frequent this process information gets updated
        /// </summary>
        public int UpdateFrequencySec { get; set; }

        public IList<Field> Information { get; set; }

        /// <summary>
        ///     Is this process up and running?
        /// </summary>
        public bool IsRunning { get; set; }
    }
}