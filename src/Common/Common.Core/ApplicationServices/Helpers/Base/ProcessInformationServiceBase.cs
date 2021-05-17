using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Common.Core.ApplicationServices.Helpers.Base
{
    public abstract class ProcessInformationServiceBase
    {
        private readonly IProcessInformationCollector[] _collectors;
        protected string ProcessInformationId;
        protected string ServerName;

        /// <summary>
        ///     Information that won't change during run time
        /// </summary>
        protected IList<Field> StaleInformation;

        protected int UpdateFrequencySec;

        protected ProcessInformationServiceBase(IProcessInformationCollector[] collectors, int updateFrequency)

        {
            _collectors = collectors;
            UpdateFrequencySec = updateFrequency;
            Init(collectors);
        }

        private void Init(IList<IProcessInformationCollector> processCollectors)
        {
            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
            ServerName = Environment.MachineName;
            
            ProcessInformationId = Directory.GetCurrentDirectory();
            StaleInformation = new List<Field>
            {
                new Field("Path",entryAssembly?.Location ?? "n/a"),
                new Field("OS", System.Runtime.InteropServices.RuntimeInformation.OSDescription),
                //new Field("ProcessName", entryAssembly.currentProcess.ProcessName),
                //new Field("Startup date", currentProcess.StartTime.ToString(CultureInfo.InvariantCulture)),
                new Field("Is 64bit OS",  System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString()),
                //new Field("Is 64bit process", Environment.Is64BitProcess.ToString())
            };
            foreach (var field in processCollectors.SelectMany(p => p.GetStaleInformation()))
            {
                StaleInformation.Add(field);
            }

            //foreach (var assembly in entryAssembly.MyGetReferencedAssembliesRecursive().Values)
            //{
            //    var assemblyName = assembly.GetName();
            //    var field = new Field("Loaded assembly",$"{assemblyName.Name} {assemblyName.Version} ({assemblyName.ProcessorArchitecture})");
            //    StaleInformation.Add(field);
            //}
           
            
        }

        /// <summary>
        ///     Collect a snapshot of this process' information, eg. RAM usage and Thread counts
        /// </summary>
        /// <returns></returns>
        private IList<Field> GetSnapshotInformation()
        {
            var info = new List<Field>();
            //var currentProcess = Process.GetCurrentProcess();
            //var memUsage = ((currentProcess.PrivateMemorySize64/1024f)/1024f);
            //info.Add(new Field("Memory usage (MB)", memUsage.ToString(CultureInfo.InvariantCulture)));
            //info.Add(new Field("Num threads", currentProcess.Threads.Count.ToString()));
            info.Add(new Field("Info", "Not yet implemented in NET standard.."));
            return info;
        }

        public ProcessInformation CreateSnapshot(string processType)
        {
            return new ProcessInformation(ProcessInformationId)
            {
                LastUpdated = DateTime.UtcNow,
                UpdateFrequencySec = UpdateFrequencySec,
                ServerName = ServerName,
                ProcessType = processType,
                Information = !_collectors.Any()
                    ? GetSnapshotInformation().Concat(StaleInformation).ToList()
                    : GetSnapshotInformation().Concat(_collectors.SelectMany(c=>c.GetSnapshotInformaiton()))
                        .Concat(StaleInformation).ToList(),
                IsRunning = true
            };
        }
    }
}