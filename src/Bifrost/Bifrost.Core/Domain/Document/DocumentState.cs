using System;

namespace Bifrost.Core.Domain.Document
{
    public class DocumentState
    {
        /// <summary>
        /// unique identifier
        /// </summary>
        public string DocumentId { get; set; }
        
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

        /// <summary>
        /// Optional data that might be needed
        /// </summary>
        public string OptionalData { get; set; }
    }
}