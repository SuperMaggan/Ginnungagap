using System;

namespace Bifrost.Dto.Dto
{
    /// <summary>
    /// Represents a job that can be executed by the jobsrunner
    /// </summary>
    public class JobDto
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

        /// <summary>
        /// Is this job currently enabled?
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        ///     When is this job to be executed
        /// eg. */2 * * * * ? will execute every other second
        ///     * */10 * * * ? will execute every tenth minute
        /// </summary>
        public string TriggerCronSyntax { get; set; }

        /// <summary>
        /// Configuration as string, this includes what kind of job you're setting up as well
        /// </summary>
        public string Configuration { get; set; }

        
        /// <summary>
        ///     When this job updated
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }

}