namespace Bifrost.Common.Core.Domain.TextExtraction
{
    /// <summary>
    /// Represents the extracted result of a binary file
    /// </summary>
    public class ExtractedDocument
    {
        public ExtractedDocument(IDocument resultingDocument)
        {
            ResultingDocument = resultingDocument;
            TextExtractionException = null;
        }

        public ExtractedDocument(IDocument resultingDocument, TextExtractionException exception)
        {
            ResultingDocument = resultingDocument;
            TextExtractionException = exception;
        }

        public ExtractedDocument(TextExtractionException exception)
        {
            ResultingDocument = null;
            TextExtractionException = exception;
        }


        /// <summary>
        /// The textual extraction result in the form of a document
        /// </summary>
        public IDocument ResultingDocument { get; set; }
        public TextExtractionException TextExtractionException { get; set; }
    }
}