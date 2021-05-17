using Quartz;

namespace Bifrost.Common.QuartzIntegration.Jobs
{
    public interface IInternalJob : IJob
    {
        void Execute();
        void Schedule(IScheduler scheduler);
    }
}