using Bifrost.Connector.Web.Infrastructure;
using Bifrost.Core.Connectors;
using Bifrost.Core.Infrastructure;
using Bifrost.Data.Sql.Infrastructure;
using Bifrost.QuartzIntegration.Infrastructure;
using Bifrost.Common.ApplicationServices.Tika.Infrastructure;
using Bifrost.Connector.SqlDatabase.Infrastructure;
using Autofac;

namespace Bifrost.JobsRunner.Infrastructure
{
  public class Bootstrapper
  {
    public static IContainer CreateContainer()
    {

      var builder = new ContainerBuilder();
      builder.RegisterModule<CoreModule>();
      builder.RegisterModule(new DocumentPipeModule(true));
      builder.RegisterModule(new JobConfigurationModule(true));
      builder.RegisterModule(new QuartzModule(true));
      builder.RegisterModule<BifrostSqlDataModule>();
      builder.RegisterModule<WebConnectorModule>();
      builder.RegisterModule<TikaTextExtractionModule>();
      builder.RegisterModule<DatabaseConnectorModule>();

      builder.RegisterType<DummyFileConnector>().As<IFileConnector>().SingleInstance();

      return builder.Build();
    }


  }
}
