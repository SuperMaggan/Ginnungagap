using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.QuartzIntegration.Jobs;
using Quartz;

namespace Bifrost.Common.Tests.TestDomain.Jobs
{
    public class SimpleJob : ConfigurableJobBase<SimpleConfig>
    {
        public SimpleJob(IJobService jobService) : base(jobService)
        {
        }

        protected override void InternalExecute(IJobExecutionContext context)
        {
            
        }
    }
}
