using System;
using System.Collections;
using System.Collections.Generic;

namespace Bifrost.Dto.Dto
{
    public class JobStateDto
    {
        /// <summary>
        /// Name of this job this state belongs to
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// A generic message describing the current state of the job
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// The last time this job was executed
        /// </summary>
        public DateTime? LastExecutionDate { get; set; }

        /// <summary>
        /// Time this job took to compelete its most recent execution (in Milliseconds) 
        /// </summary>
        public int LastExecutionDurationMs { get; set; }

        /// <summary>
        /// Recent error
        /// </summary>
        public ErrorDto Error { get; set; }

        /// <summary>
        /// Current status of this job
        /// </summary>
        public JobStatusDto Status { get; set; }

        public JobProgressDto Progress { get; set; }

        public IList<FieldDto> Custom { get; set; }

    }
}