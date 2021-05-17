using Bifrost.Common.Core.Domain.Jobs;

namespace Bifrost.Core.Connectors.Configs
{
    public class FileConnectorJobConfiguration :JobConfigurationBase
    {
        public override string Description => "Extracts textual data from files found on the file system [EXPERIMENTAL]";
        public override string DisplayName => "File crawler";
        /// <summary>
        /// Where to start the crawl
        /// </summary>
        public string RootDirectory { get; set; }

        /// <summary>
        /// Should the crawler follow directories in the root?
        /// </summary>
        public bool CrawlRecursively { get; set; }

        /// <summary>
        /// How big each batch of documents can be at most (in kB)
        /// </summary>
        public int BatchSizeLimit { get; set; }


        public override IJobConfiguration CreateExample()
        {
            return new FileConnectorJobConfiguration()
            {
                RootDirectory = "D:\\Documents",
                BatchSizeLimit = 100,
                CrawlRecursively = true
            };
        }

    }
}