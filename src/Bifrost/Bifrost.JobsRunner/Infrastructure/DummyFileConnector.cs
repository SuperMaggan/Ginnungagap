using Bifrost.Core.Connectors;
using Bifrost.Core.Connectors.Configs;
using Bifrost.Core.Domain;

namespace Bifrost.JobsRunner.Infrastructure
{
    public class DummyFileConnector : IFileConnector
    {
        public SourceChanges ExecuteFetch(FileConnectorJobConfiguration config)
        {
            throw new System.NotImplementedException();
        }

        public void ResetConnector(string jobName)
        {
            throw new System.NotImplementedException();
        }
    }
}