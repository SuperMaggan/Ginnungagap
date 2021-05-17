using System;
using System.Collections.Generic;
using System.Linq;
using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Pipeline.Interfaces;
using Bifrost.Core.Settings;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Core.ApplicationServices.Implementations
{
    public class ProcessInformationServiceBase
    {
        protected CommonSettings CommonSettings;
        private readonly IIntegrationService[] _integrationServices;
        private readonly ITextExtractor[] _extractors;
        private readonly IDocumentPipelineStep[] _documentPipelineSteps;
        private readonly IDocumentBatchPipelineStep[] _documentBatchPipelineSteps;
        private ProcessInformation _processInformationBase;
        private IList<Field> _staleInformation;

        public ProcessInformationServiceBase(CommonSettings commonSettings,
            IIntegrationService[] integrationServices,
            ITextExtractor[] extractors,
            IDocumentPipelineStep[] documentPipelineSteps, 
            IDocumentBatchPipelineStep[] documentBatchPipelineSteps)
        {
            CommonSettings = commonSettings;
            _integrationServices = integrationServices;
            _extractors = extractors;
            _documentPipelineSteps = documentPipelineSteps;
            _documentBatchPipelineSteps = documentBatchPipelineSteps;

            Init();
        }
        
        private void Init()
        {

            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
            //var currentProcess = Process.GetCurrentProcess();
            _processInformationBase = new ProcessInformation(entryAssembly?.Location ?? "n/a")
            {
                ProcessType = "Bifrost.JobsRunner",
                ServerName = Environment.MachineName,
                UpdateFrequencySec = 15
            };

            _staleInformation = new List<Field>();
            foreach (var integration in _integrationServices)
            {
                _staleInformation.Add(new Field("Dispatcher", integration.ToString()));
            }
            foreach (var extractor in _extractors)
            {
                _staleInformation.Add(new Field("Extractor", extractor.ToString()));
            }
            foreach (var step  in _documentPipelineSteps)
            {
                _staleInformation.Add(new Field("PipelineStep", step.ToString()));
            }
            foreach (var step in _documentBatchPipelineSteps)
            {
                _staleInformation.Add(new Field("PipelineBatchStep", step.ToString()));
            }
            //_staleInformation.Add(new Field("Path", Environment.CurrentDirectory));
            //_staleInformation.Add(new Field("OS", Environment.OSVersion.ToString()));
            _staleInformation.Add(new Field("DateFormat", CommonSettings.DateTimeFormat));
            _staleInformation.Add(new Field("Cache", CommonSettings.UseFileSystemAsFailover 
                ? "Yes, at " + CommonSettings.FileSystemDirectory : "No" ));
            //_staleInformation.Add(new Field("ProcessName", currentProcess.ProcessName));
            //_staleInformation.Add(new Field("Startup date", currentProcess.StartTime.ToString(CultureInfo.InvariantCulture)));
;
        }

        private IList<Field> GetSnapshotInformation()
        {
            var info = new List<Field>();

            //var currentProcess = Process.GetCurrentProcess();
            //var memUsage = ((currentProcess.PrivateMemorySize64/1024f)/1024f);
            //info.Add(new Field("Memory usage (MB)", memUsage.ToString(CultureInfo.InvariantCulture)));
            //info.Add(new Field("Num threads", currentProcess.Threads.Count.ToString()));
            info.Add(new Field("Num threads", "Hopefully coming in a later netstandard version.."));
            return info;
        }

        protected ProcessInformation GetCurrentProcessInformation()
        {
            return new ProcessInformation(_processInformationBase.Id)
            {
                ProcessType = _processInformationBase.ProcessType,
                LastUpdated = DateTime.UtcNow,
                ServerName = _processInformationBase.ServerName,
                UpdateFrequencySec = _processInformationBase.UpdateFrequencySec,

                Information = _staleInformation.Concat(GetSnapshotInformation()).ToList()
            };
        }
    }
}