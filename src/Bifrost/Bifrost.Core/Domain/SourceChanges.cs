using System.Collections.Generic;
using System.Linq;
using Bifrost.Core.Domain.Document;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Core.Domain
{
    /// <summary>
    /// Represents any changes made to a source system
    /// </summary>
    public class SourceChanges
    {
        public SourceChanges()
        {
            Adds = new List<AddDocument>();
            Deletes = new List<DeleteDocument>();
        }
        public SourceChanges(IList<AddDocument> adds, IList<DeleteDocument> deletes)
        {
            Adds = adds;
            Deletes = deletes;
        }
        public SourceChanges(IList<AddDocument> adds, IEnumerable<Field> deletes)
        {
            Adds = adds;
            Deletes = deletes.Select(x => new DeleteDocument(x.Name, x.Value)).ToList();
        }

        public SourceChanges(IList<IDocument> documents)
        {
            Adds = documents
                .Where(d => d is AddDocument)
                .Cast<AddDocument>()
                .ToList();
            Deletes = documents
                .Where(d => d is DeleteDocument)
                .Cast<DeleteDocument>()
                .ToList();

        }

        /// <summary>
        /// Added or updated documents
        /// </summary>
        public IList<AddDocument> Adds { get; private set; }

        /// <summary>
        /// Documents thats been deleted
        /// </summary>
        public IList<DeleteDocument> Deletes { get; private set; }
    }
}