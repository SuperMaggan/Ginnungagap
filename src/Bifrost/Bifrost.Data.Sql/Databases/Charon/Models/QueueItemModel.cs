using System;
using Dapper.Contrib.Extensions;
using Bifrost.Data.Sql.Databases.Bifrost.Objects.Tables;

namespace Bifrost.Data.Sql.Databases.Bifrost.Models
{
    [Table("QueueItems")]
    public class QueueItemModel
    {
        /// <summary>
        /// Identifier for this entry 
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Creation date for this entry
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