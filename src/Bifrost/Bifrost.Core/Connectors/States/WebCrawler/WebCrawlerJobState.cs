using Bifrost.Common.Core.Domain;

namespace Bifrost.Core.Connectors.States.WebCrawler
{
    public class WebCrawlerJobState: ConnectorStateBase
    {
        public WebCrawlerJobState()
        {

        }

        public WebCrawlerJobState(State state)
        {
            Name = state.Name;
            Fields = state.Fields;
        }
       

    }
}