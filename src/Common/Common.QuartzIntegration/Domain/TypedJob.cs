using System;
using Bifrost.Common.Core.Domain.Jobs;

namespace Bifrost.Common.QuartzIntegration.Domain
{
    /// <summary>
    /// Represents a job that has predefined what class is supposed
    /// to execute it (as opposed to the executing job deciding if it can handle a job's IJobConfiguration type)
    /// </summary>
    public class TypedJob : Job
{
        
        public Type JobType { get; set; }
}
}
