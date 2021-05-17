using Bifrost.Core.ApplicationServices.Implementations;
using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Settings;
using Bifrost.Common.Core.ApplicationServices;
using Autofac;

namespace Bifrost.Core.Infrastructure
{
    public class CoreModule : Module
    {
      protected override void Load(ContainerBuilder builder)
      {
        builder.RegisterType<AsgardProcessInformationCollector>().As<IProcessInformationCollector>().SingleInstance();
        builder.RegisterType<MirrorIntegrationCoordinatorService>().As<IIntegrationCoordinatorService>().SingleInstance();
        builder.RegisterType<FileSystemDocumentCache>().As<IDocumentCache>().SingleInstance();
        builder.RegisterType<FileSystemIntegrationService>().As<IIntegrationService>().SingleInstance();
        builder.RegisterType<FileSystemDocumentErrorHandler>().As<IDocumentErrorHandler>().SingleInstance();
        builder.RegisterType<CommonSettings>().AsSelf().SingleInstance();
      }
    }
}