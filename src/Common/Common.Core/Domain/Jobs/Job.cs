using System;

namespace Bifrost.Common.Core.Domain.Jobs
{
    public class Job
    {
        /// <summary>
        ///     Identifier for this job
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     What is this job doing?
        /// </summary>
        public string Description { get; set; }

        //public string JobType { get; set; }

        /// <summary>
        ///     Number of jobs that canbe run simultaneously
        /// </summary>
        public int ConcurrentLimit { get; set; }

        public bool Enabled { get; set; }

        /// <summary>
        ///     When is this job to be executed
        /// </summary>
        public string TriggerCronSyntax { get; set; }

        /// <summary>
        /// </summary>
        public IJobConfiguration Configuration { get; set; }

        /// <summary>
        ///     When this job updated
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }
}