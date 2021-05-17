using Bifrost.Core.Settings;

namespace Bifrost.Integration.Solr
{
    public class SolrSettings : AsgardSettingsBase
    {
        /// <summary>
        /// Where to send the documents
        /// <example>servername:port</example>
        /// </summary>
        public string SolrUpdateUrl { get; set; }
        /// <summary>
        /// Url that should be used for testing if solr is up and running
        /// </summary>
        public string SolrPingUrl { get; set; }

        /// <summary>
        /// Send a /commit element with each dispatch to Solr
        /// </summary>
        public bool SolrIncludeCommit { get; set; }

        /// <summary>
        /// What field in the solr setup that holds Id
        /// </summary>
        public string IdFieldName { get; set; } = "id";

    }
}