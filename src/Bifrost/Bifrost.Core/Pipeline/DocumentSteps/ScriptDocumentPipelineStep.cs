using Bifrost.Core.Pipeline.Interfaces;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Core.Pipeline.DocumentSteps
{
    public class ScriptDocumentPipelineStep : IDocumentPipelineStep
    {
        private readonly IScriptProcessingService _scriptProcessingService;

        public ScriptDocumentPipelineStep(IScriptProcessingService scriptProcessingService)
        {
            _scriptProcessingService = scriptProcessingService;
        }

        public IDocument Invoke(IDocument document, string domain)
        {
            return _scriptProcessingService.Process(document);
        }

        public bool Enabled { get; set; }
    }
}