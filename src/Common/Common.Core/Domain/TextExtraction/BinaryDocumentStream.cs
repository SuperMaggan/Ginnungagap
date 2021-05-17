using System.IO;

namespace Bifrost.Common.Core.Domain.TextExtraction
{
    /// <summary>
    /// Represents a binary file serialized to a stream
    /// </summary>
    public class BinaryDocumentStream
    {
        public BinaryDocumentStream(string documentId,string domain, Stream stream)
        {
            Id = documentId;
            Stream = stream;
            Domain = domain;
        }


        /// <summary>
        /// The string value representing this very document
        /// Will be used in a later stages if you need to delete the document
        /// </summary>
        public string Id { get; set; }


        public string Domain { get; set; }

        /// <summary>
        /// The actual document stream
        /// </summary>
        public Stream Stream { get; set; }
    }
}