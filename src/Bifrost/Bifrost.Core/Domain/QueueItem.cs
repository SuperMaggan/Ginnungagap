using System;

namespace Bifrost.Core.Domain
{
    public class QueueItem
    {
        public QueueItem()
        {
            
        }

        public QueueItem(string documentId)
        {
            CreateDate = DateTime.Now;
            DocumentId = documentId;
            OptionalData = null;
        }
        /// <summary>
        /// When this item was placed in queue
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// eg. a document id or url 
        /// </summary>
        public string DocumentId { get; set; }

        /// <summary>
        /// Any optional data that is needed
        /// </summary>
        public string OptionalData { get; set; } 
    }
}