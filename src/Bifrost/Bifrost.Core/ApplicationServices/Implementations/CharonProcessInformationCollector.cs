using System.Collections.Generic;
using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Pipeline.Interfaces;
using Bifrost.Core.Settings;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Core.ApplicationServices.Implementations
{
    public class AsgardProcessInformationCollector : IProcessInformationCollector {
        private readonly CommonSettings _commonSettings;
        private readonly IIntegrationService[] _integrationServices;
        private readonly ITextExtractor[] _extractors;
        private readonly IDocumentPipelineStep[] _documentPipelineSteps;
        private readonly IDocumentBatchPipelineStep[] _documentBatchPipelineSteps;

        public AsgardProcessInformationCollector(CommonSettings commonSettings,
            IIntegrationService[] integrationServices,
            ITextExtractor[] extractors,
            IDocumentPipelineStep[] documentPipelineSteps, 
            IDocumentBatchPipelineStep[] documentBatchPipelineSteps)
        {
            _commonSettings = commonSettings;
            _integrationServices = integrationServices; 
             _extractors = extractors;
            _documentPipelineSteps = documentPipelineSteps;
            _documentBatchPipelineSteps = documentBatchPipelineSteps;
        }

        public IList<Field> GetStaleInformation()
        {
            var staleInfo = new List<Field>();
            foreach (var integration in _integrationServices)
            {
                staleInfo.Add(new Field("Dispatcher", integration.ToString()));
            }
            foreach (var extractor in _extractors)
            {
                staleInfo.Add(new Field("Extractor", extractor.ToString()));
            }
            foreach (var step in _documentPipelineSteps)
            {
                staleInfo.Add(new Field("PipelineStep", step.ToString()));
            }
            foreach (var step in _documentBatchPipelineSteps)
            {
                staleInfo.Add(new Field("PipelineBatchStep", step.ToString()));
            }

            staleInfo.Add(new Field("DateFormat", _commonSettings.DateTimeFormat));
            staleInfo.Add(new Field("Cache", _commonSettings.UseFileSystemAsFailover
                ? "Yes, at " + _commonSettings.FileSystemDirectory : "No"));

         


            return staleInfo;
        }


        public IList<Field> GetSnapshotInformaiton()
        {
            return new Field[0];
        }
    }
}