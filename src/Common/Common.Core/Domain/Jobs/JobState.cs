using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bifrost.Common.Core.Domain.Jobs
{
    public class JobState : ReflectionStateBase
    {
        public JobState()
        {
            
        }

        public JobState(State state)
        {
            Name = state.Name;
            Fields = state.Fields;
        }
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
        public int LastExecutionTimeMs { get; set; }

        /// <summary>
        /// The most recent error message 
        /// </summary>
        public string RecentErrorMessage { get; set; }
        /// <summary>
        /// The most recent error stacktrace
        /// </summary>
        public string RecentErrorStackTrace { get; set; }

        /// <summary>
        /// When the most recent error occurred
        /// </summary>
        public DateTime? RecentErrorDate { get; set; }

        /// <summary>
        /// Number of consecutive occuring errors
        /// </summary>
        public int ErrorCount { get; set; }
        /// <summary>
        /// Current status of this job
        /// </summary>
        public JobStatus Status { get; set; }
        
        
        public virtual void SetErrorState(Exception e)
        {
            RecentErrorMessage = e.Message;
            RecentErrorStackTrace = e.StackTrace;
            RecentErrorDate = DateTime.UtcNow;
            Status = JobStatus.Error;
            ErrorCount++;
        }

        public virtual JobError GetJobError()
        {
            if (!RecentErrorDate.HasValue)
                return null;
            return new JobError()
            {
                Date = RecentErrorDate.Value,
                ErrorCount = ErrorCount,
                Message = RecentErrorMessage,
                StackTrace = RecentErrorStackTrace
            };
        }

        /// <summary>
        /// Get all fields that does not have a property value 
        /// </summary>
        /// <returns></returns>
        public virtual IList<Field> GetCustomFields()
        {
            var propertyNames = GetType().GetTypeInfo().GetProperties().Select(x=>x.Name).ToArray();
            return Fields.Where(x => !propertyNames.Contains(x.Name))
                .ToList();
        }
    }
}