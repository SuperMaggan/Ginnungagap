using Bifrost.Core.Connectors.Configs;
using Bifrost.Core.Connectors.Configs.SqlDatabase;
using Bifrost.Core.Connectors.Configs.Web;
using Bifrost.Core.Pipeline.Interfaces;
using Bifrost.Common.Core.ApplicationServices.Helpers;
using Bifrost.Common.Core.Domain.Jobs;
using Autofac;

namespace Bifrost.Core.Infrastructure
{

  public class JobConfigurationModule : Module
  {
    private readonly bool registerJobs;

    public JobConfigurationModule() : this(true)
    {

    }
    public JobConfigurationModule(bool registerJobs)
    {
      this.registerJobs = registerJobs;
    }

    protected override void Load(ContainerBuilder builder)
    {
      if (registerJobs)
      {
        builder.RegisterType<FileConnectorJobConfiguration>().As<IJobConfiguration>().SingleInstance();
        builder.RegisterType<SqlDatabaseConnectorJobConfiguration>().As<IJobConfiguration>().SingleInstance();
        builder.RegisterType<WebConnectorJobConfiguration>().As<IJobConfiguration>().SingleInstance();
      }

      builder.RegisterType<DefaultJobConfigurationMapper>().As<IJobConfigurationMapper>().SingleInstance();
    }
  }

}