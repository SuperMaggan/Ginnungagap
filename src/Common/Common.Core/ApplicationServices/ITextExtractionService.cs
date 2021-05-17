using System.Collections.Generic;
using Bifrost.Common.Core.Domain.TextExtraction;

namespace Bifrost.Common.Core.ApplicationServices
{
    /// <summary>
    /// Provides an interface for extracting raw text from binary data
    /// eg. PDF, doc, vsd etc. 
    /// </summary>
    public interface ITextExtractionService
    {
        /// <summary>
        /// Extract text from one or several files
        /// Each file will result in a new Document object
        /// </summary>
        /// <returns>If any exception is thrown the ExtractedDocument will contain that Exception</returns>
        IEnumerable<ExtractedDocument> ExtractText(IList<BinaryDocumentFile> documentFiles);

        /// <summary>
        /// Extract text from one or several streams
        /// </summary>
        /// <returns>If any exception is thrown the ExtractedDocument will contain that Exception</returns>
        IEnumerable<ExtractedDocument> ExtractText(IList<BinaryDocumentStream> documentStreams);

        /// <summary>
        /// Extract text from one files
        /// </summary>
        /// <returns>If any exception is thrown the ExtractedDocument will contain that Exception</returns>
        ExtractedDocument ExtractText(BinaryDocumentFile documentFile);

        /// <summary>
        /// Extract text from one stram
        /// </summary>
        /// <returns>If any exception is thrown the ExtractedDocument will contain that Exception</returns
        ExtractedDocument ExtractText(BinaryDocumentStream documentStream);
    }
}