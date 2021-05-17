using System.Collections.Generic;

namespace Bifrost.Core.Domain.Document
{
    /// <summary>
    /// Describes how a document relates to one or more other documents
    /// eg. A page links to 10 other pages
    /// eg. A Document references 4 other documents
    /// </summary>
    public class DocumentRelation
    {
        /// <summary>
        /// The identifier for this document
        /// </summary>
        public string DocumentId { get; set; }

        /// <summary>
        /// Other document Ids that this document has a relation to  
        /// </summary>
        public IList<string> Relations { get; set; }

    }


}
