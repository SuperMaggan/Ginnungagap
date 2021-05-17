using System;

namespace Bifrost.Connector.Web.Domain
{
    public class PageQueueItem
    {
        
        /// <summary>
        /// Usually the same as Url unless something else defines a page, eg. a Canonical link
        /// </summary>
        public string Id { get; set; }

        public Uri Url { get; set; }

        public int Depth { get; set; }
        /// <summary>
        /// When this item was placed in queue
        /// </summary>
        public DateTime CreateDate { get; set; }





    }
}