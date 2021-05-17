using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Bifrost.Core.Pipeline.Interfaces;

namespace Bifrost.Processing.CsScript.Infrastructure {
 
    public class CsScriptProcessorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {

            container.Register(
                Component.For<CsScriptProcessingService>()
                    .ImplementedBy<CsScriptProcessingService>()
                    .LifestyleSingleton(),
                    Component.For<IDocumentBatchPipelineStep>()
                    .ImplementedBy<CsScriptProcessingDocumentBatchStep>()
                    .LifestyleSingleton()
                );        
        }
    }
}