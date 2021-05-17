using Autofac;
using Bifrost.Connector.SqlDatabase.DatabaseConnectorState;
using Bifrost.Connector.SqlDatabase.DatabaseIntegration;
using Bifrost.Connector.SqlDatabase.SqlServer;
using Bifrost.Core.Connectors;

namespace Bifrost.Connector.SqlDatabase.Infrastructure
{
    public class DatabaseConnectorModule : Module 
    {
       
      protected override void Load(ContainerBuilder builder)
      {
        builder.RegisterType<SqlDatabaseConnector>().As<ISqlDatabaseConnector>().SingleInstance();
        builder.RegisterType<SqlServerIntegration>().As<IDatabaseIntegration>().SingleInstance();
        builder.RegisterType<EventTableDataChangeDiscoverer>().As<IDataChangeDiscoverer>().SingleInstance();
        builder.RegisterType<ChangetableDataChangeDiscoverer>().As<IDataChangeDiscoverer>().SingleInstance();
      }
  }
}