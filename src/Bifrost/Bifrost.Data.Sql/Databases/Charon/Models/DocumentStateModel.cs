using System;
using Dapper.Contrib.Extensions;
using Bifrost.Data.Sql.Databases.Bifrost.Objects.Tables;

namespace Bifrost.Data.Sql.Databases.Bifrost.Models
{
    [Table("DocumentStates")]
    public class DocumentStateModel
    {
        /// <summary>
        /// The Category that this document is tied to
        /// </summary>
        [ExplicitKey]
        public string Category { get; set; }

        /// <summary>
        /// unique identifier
        /// </summary>
        [ExplicitKey]
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
        /// When this document state was last verified
        /// </summary>
        public DateTime LastVerified { get; set; }

        public string OptionalData { get; set; }


        /// <summary>
        /// WHen this document should be checked for updates again
        /// </summary>
        public DateTime VerifyDate { get; set; }
    }

}