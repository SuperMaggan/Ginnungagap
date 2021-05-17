using Bifrost.QuartzIntegration.Jobs.Internal;
using Bifrost.Common.QuartzIntegration.Infrastructure;
using Bifrost.Common.QuartzIntegration.Jobs;
using Bifrost.Common.QuartzIntegration.Jobs.Internal;
using Autofac;

namespace Bifrost.QuartzIntegration.Infrastructure
{
  public class QuartzModule : Module
  {
    private readonly bool registerConnectorJobs;

    public QuartzModule() : this(true)
    {

    }
    public QuartzModule(bool registerConnectorJobs)
    {
      this.registerConnectorJobs = registerConnectorJobs;
    }

    protected override void Load(ContainerBuilder builder)
    {
      builder.RegisterModule<CommonQuartzModule>();
      builder.RegisterType<UpdateInMemoryQuartzJob>()
        .AsSelf()
        .As<IInternalJob>()
        .SingleInstance();
      builder.RegisterType<UpdateProcessInformationJob>()
        .AsSelf()
        .As<IInternalJob>()
        .SingleInstance();
      builder.RegisterType<ResumePausedJobsJob>()
        .AsSelf()
        .As<IInternalJob>()
        .SingleInstance();

      if (registerConnectorJobs)
      {
        builder.RegisterAssemblyTypes(System.Reflection.Assembly.GetAssembly(this.GetType()))
            .AssignableTo<IConfigurableJob>()
            .AsImplementedInterfaces()
            .SingleInstance();
      }

    }

  }

}