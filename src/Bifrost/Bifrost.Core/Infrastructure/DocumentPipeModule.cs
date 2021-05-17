using Bifrost.Core.ApplicationServices.Implementations;
using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Pipeline.DocumentSteps;
using Bifrost.Core.Pipeline.Implementations;
using Bifrost.Core.Pipeline.Interfaces;
using Bifrost.Common.Core.ApplicationServices;
using Autofac;

namespace Bifrost.Core.Infrastructure
{
  public class DocumentPipeModule : Module
  {
    private readonly bool registerSteps;

    public DocumentPipeModule(bool registerSteps)
    {
      this.registerSteps = registerSteps;
    }

    protected override void Load(ContainerBuilder builder)
    {
      if (registerSteps)
      {
        builder.RegisterType<LimitDocumentSizePipelineStep>().As<IDocumentPipelineStep>().SingleInstance();
        builder.RegisterType<BinaryDataExtractionPipelineStep>().As<IDocumentPipelineStep>().SingleInstance();
        builder.RegisterType<SoftDeletePipelineStep>().As<IDocumentPipelineStep>().SingleInstance();
      }

      builder.RegisterType<SerialDocumentPipeline>().As<IDocumentPipeline>().SingleInstance();
      builder.RegisterType<MirrorIntegrationCoordinatorService>().As<IIntegrationCoordinatorService>().SingleInstance();
      builder.RegisterType<AsgardProcessInformationCollector>().As<IProcessInformationCollector>().SingleInstance();
    }
  }
}   