using System;

namespace Bifrost.Common.Core.Domain.TextExtraction
{
    /// <summary>
    /// Exceptions thrown by a TextExctractor when an extraction process somehow went wrong
    /// </summary>
    public class TextExtractionException : Exception
    {
        public string DocumentId { get; set; }
        public TextExtractionException(string documentId, string message) :
            base(message)
        {
            DocumentId = documentId;
        }

        public TextExtractionException(string documentId, string message, Exception innerException) :
            base(message, innerException)
        {
            DocumentId = documentId;
        }


        public override string ToString()
        {
            if (InnerException == null)
                return string.Format("Document Id: {0}\r\n {1}", DocumentId, Message);
            return string.Format("Document Id: {0}\r\n{1}\r\n\r\n {2}", DocumentId, Message, InnerException.ToString());

        }

    }
}