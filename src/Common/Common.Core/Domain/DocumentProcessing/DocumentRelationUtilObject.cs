using System;
using System.Collections.Generic;
using System.Linq;

namespace Bifrost.Common.Core.Domain.DocumentProcessing
{
    /// <summary>
    /// Used for describing a one to many relation between documents
    /// </summary>
    public class DocumentRelationUtilObject : IProcessingUtilObject
    {
        public DocumentRelationUtilObject()
        {
            DocumentIds = new List<string>();
        }
        public string Key { get; set; }

        public IList<string> DocumentIds;


        private const string Delimiter = "<!>";
        public string Serialize()
        {
            return string.Join((string) Delimiter, (IEnumerable<string>) DocumentIds);
        }

        public void Deserialize(string key, string objectStr)
        {
            var tokens = objectStr.Split(new string[] { Delimiter }, StringSplitOptions.RemoveEmptyEntries);
            Key = key;
            DocumentIds = tokens.ToList();
        }
    }
}