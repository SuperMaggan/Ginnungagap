namespace Bifrost.Core.Connectors.Configs.Web
{
    /// <summary>
    /// A collection of rules that defines if a page should be included or not
    /// </summary>
    public class LinkFilter
    {
        public LinkFilter()
        {
            StayOnHost = true;
            ExcludeExtensions = new string[0];
            ExcludeRegex = new string[0];
            IncludeRegex = new string[0];

        }
        /// <summary>
        /// If true, a page that does not match one of the include rules will be excluded
        /// </summary>
        public bool ExcludeAsDefault { get; set; }

        /// <summary>
        /// Pages with the defined extensions will be excluded
        /// </summary>
        public string[] ExcludeExtensions { get; set; }

        /// <summary>
        /// Pages' URL that matches at least one of the defined regex will be excluded
        /// </summary>
        public string[] ExcludeRegex { get; set; }

        /// <summary>
        /// Pages' URL that matches at least one of the defined regex will be included
        /// </summary>
        public string[] IncludeRegex { get; set; }

        /// <summary>
        /// Exclude pages which URL contains query parameters
        /// </summary>
        public bool ExcludePagesWithQueryParameters { get; set; }

        /// <summary>
        /// if true then only crawl pages from the same host as the start url
        /// </summary>
        public bool StayOnHost { get; set; }
    }
}