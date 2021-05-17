using Bifrost.Common.Core.Domain.Jobs;

namespace Bifrost.Common.QuartzIntegration.Helpers
{
    public static class QuartzExtensions
    {
        public static string TriggerName(this Job job)
        {
            return string.Format("{0}TRIGGER", job.Name);

        }

        
    }
}
