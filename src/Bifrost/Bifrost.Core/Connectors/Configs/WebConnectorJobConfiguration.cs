using Bifrost.Core.Connectors.Configs.Common;
using Bifrost.Core.Connectors.Configs.Web;
using Bifrost.Core.Domain;
using Bifrost.Common.Core.Domain.Jobs;

namespace Bifrost.Core.Connectors.Configs
{
    public class WebConnectorJobConfiguration : JobConfigurationBase
    {
        public override string Description => "Extracts textual data from web sites and binary documents";
        public override string DisplayName => "Web connector";
        public string StartUrl { get; set; }
        public int Depth { get; set; }

        public int NumberOfPagesPerExecution { get; set; }

        //[Description("Include the raw Html for each page")]
        //public bool IncludeSourceHtml { get; set; }

        public LinkFilter LinkFilter { get; set; } = new LinkFilter();
        public PageFilter PageFilter { get; set; } = new PageFilter();
        public Credential Credential { get; set; }

        public Frequency DefaultVerifyFrequency { get; set; }

        public override IJobConfiguration CreateExample()
        {
            return new WebConnectorJobConfiguration()
            {
                StartUrl = "http://blog.cwa.me.uk",
                Depth = 3,
                LinkFilter = new LinkFilter()
                { 
                    ExcludeExtensions = new []
                    {
                        "js","css","swf","img","png","jpg"
                    },
                    ExcludeAsDefault = false,
                    ExcludePagesWithQueryParameters = true
                },
                PageFilter = new PageFilter()
                {
                    ExcludeBinaryPages = false
                },
                NumberOfPagesPerExecution = 10,
                DefaultVerifyFrequency = new Frequency()
                {
                    Days = 1
                },
                Credential= new Credential()
                {
                    Domain = "FKA",
                    Username = "8tm",
                    Password = "iejr"

                }
            };
        }


    }
}