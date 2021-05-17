using System.Collections.Generic;
using System.Linq;
using Bifrost.Core.Domain.Document;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Core.Utils
{
    public static class DocumentUtils
    {
        public static void MergeWithDocuments(this IList<AddDocument> mainDocuments, IList<AddDocument> candidates, KeyValuePair<string, string> relation, string prefix)
        {
            foreach (var mainDocument in mainDocuments)
            {
                mainDocument.MergeWithDocuments(candidates, relation, prefix);
            }
        }

        public static void MergeWithDocuments(this AddDocument mainDocument, IList<AddDocument> candidates, KeyValuePair<string, string> relation, string prefix)
        {
            var documentsToMerge = SelectRelevantForeignDocuments(candidates, relation.Value, mainDocument.GetFieldValue(relation.Key));
            int i = 0;
            foreach (var document in documentsToMerge)
            {
                foreach (var field in document.Fields)
                {
                    mainDocument.Fields.Add(new Field($"{prefix}{i}_{field.Name}", field.Value));
                }
                i++;
            }
        }


        private static IEnumerable<AddDocument> SelectRelevantForeignDocuments(IList<AddDocument> documents, string fieldName, string fieldValue)
        {
            return from document in documents 
                   let fields = document.Fields.Where(x => x.Name.Equals(fieldName))
                   where fields.Any(field => field.Value.Equals(fieldValue)) 
                   select document;
        }
    }
}
