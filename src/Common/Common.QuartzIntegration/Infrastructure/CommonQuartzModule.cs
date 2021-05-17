using Autofac;
using Autofac.Extras.Quartz;
using Bifrost.Common.QuartzIntegration.Server;
using Quartz;
using Quartz.Impl;
using Quartz.Job;
using Quartz.Spi;

namespace Bifrost.Common.QuartzIntegration.Infrastructure
{
    public class CommonQuartzModule : Module
    {

    protected override void Load(ContainerBuilder builder)
    {
      //builder.RegisterType<AutofacJobFactory>().As<IJobFactory>().SingleInstance();
      builder.RegisterModule(new QuartzAutofacFactoryModule());
      builder.RegisterType<GlobalJobListener>().As<IJobListener>().SingleInstance();
      builder.RegisterType<QuartzServer>().As<IQuartzServer>().SingleInstance();
      builder.RegisterType<StdSchedulerFactory>().As<ISchedulerFactory>().SingleInstance();
      builder.RegisterType<XmlFileQuartzJobConfigurator>().As<IQuartzJobConfigurator>().SingleInstance();
      builder.RegisterType<FileScanJob>().AsSelf();
    }
  }
}
