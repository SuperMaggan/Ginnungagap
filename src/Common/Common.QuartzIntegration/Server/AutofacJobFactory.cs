using Autofac;
using Quartz;
using Quartz.Simpl;
using Quartz.Spi;
using Serilog;
namespace Bifrost.Common.QuartzIntegration.Server
{
    public class AutofacJobFactory : IJobFactory
    {
        private readonly IContainer _container;

        public AutofacJobFactory(IContainer container)
        {
            _container = container;
        }
        
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _container.Resolve(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
           Log.Debug($"Returned job {job.GetType().Name}");
        }
    }
}