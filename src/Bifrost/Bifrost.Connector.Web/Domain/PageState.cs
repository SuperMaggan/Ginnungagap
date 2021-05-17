using System;

namespace Bifrost.Connector.Web.Domain
{
    public class PageState
    {
        /// <summary>
        /// Usually the same as Url unless something else defines a page, eg. a Canonical link
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// unique identifier
        /// </summary>
        public Uri Url { get; set; }

        

        /// <summary>
        /// Hash value, used to detect any changes
        /// </summary>
        public string HashValue { get; set; }

        /// <summary>
        /// When this document state was last updated
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Last time this document was verified for updates
        /// </summary>
        public DateTime LastVerified { get; set; }

        /// <summary>
        /// WHen this document should be checked for updates again
        /// </summary>
        public DateTime VerifyDate { get; set; }

        public int Depth { get; set; }


    }
    
}