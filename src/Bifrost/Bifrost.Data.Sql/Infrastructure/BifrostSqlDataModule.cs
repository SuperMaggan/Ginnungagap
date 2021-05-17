using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Document;
using Bifrost.Data.Sql.Databases;
using Bifrost.Data.Sql.Databases.Bifrost.Mappers;
using Bifrost.Data.Sql.Databases.Bifrost.Models;
using Bifrost.Common.ApplicationServices.Sql;
using Bifrost.Common.ApplicationServices.Sql.Common;
using Bifrost.Common.ApplicationServices.Sql.Databases;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.ApplicationServices.Helpers;
using Autofac;

namespace Bifrost.Data.Sql.Infrastructure
{
  public class BifrostSqlDataModule : Module
  {

    protected override void Load(ContainerBuilder builder)
    {
      var settings = new SqlSettings();
      builder.RegisterInstance(settings);

      //Mappers
      builder.RegisterType<QueueItemMapper>().As<IMapper<QueueItem, QueueItemModel>>()
        .SingleInstance();
      builder.RegisterType<DocumentStateMapper>().As<IMapper<DocumentState, DocumentStateModel>>()
        .SingleInstance();

      //Database
      builder.RegisterType<AsgardDatabase>()
        .SingleInstance();

      //Services
      builder.RegisterType<SqlQueueService>().As<IQueueService>().SingleInstance();
      builder.RegisterType<SqlDocumentStateService>().As<IDocumentStateService>().SingleInstance();
      builder.RegisterType<SqlConsumerService>().As<IConsumerService>().SingleInstance();

      builder.RegisterType<SqlWorkTaskService>().As<IWorkTaskService>().SingleInstance();
      builder.RegisterType<SqlStateService>().As<IStateService>().SingleInstance();
      builder.RegisterType<SqlProcessInformationService>().As<IProcessInformationService>().SingleInstance();
      builder.RegisterType<SqlJobService>().As<IJobService>().SingleInstance();
      
      //Common database components
      builder.RegisterInstance(new CommonDatabaseSettings()
      {
        CommonConnection = settings.BifrostConnection,
        DestroyCommonDatabase = settings.DestroyBifrostDatabase
      }).SingleInstance();

      builder.RegisterType<WorkTaskDatabase>().AsSelf().SingleInstance();
      builder.RegisterType<StateDatabase>().AsSelf().SingleInstance();
      builder.RegisterType<ProcessInformationDatabase>().AsSelf().SingleInstance();
      builder.RegisterType<JobDatabase>().AsSelf().SingleInstance();

      builder.RegisterType<DefaultJobConfigurationMapper>().As<IJobConfigurationMapper>().SingleInstance();
    }
  }
}