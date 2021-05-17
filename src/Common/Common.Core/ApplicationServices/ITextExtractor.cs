using Bifrost.Common.Core.Domain;
using Bifrost.Common.Core.Domain.TextExtraction;

namespace Bifrost.Common.Core.ApplicationServices
{
    /// <summary>
    /// Provides an interface for an implementation that extracts text
    /// from one or several types of binary files
    /// </summary>
    public interface ITextExtractor
    {
        /// <summary>
        /// Extract text from a file
        /// </summary>
        /// <returns></returns>
        IDocument ExtractText(BinaryDocumentFile binaryDocumentFile);

        /// <summary>
        /// Extracts text from a stream
        /// </summary>
        /// <returns></returns>
        IDocument ExtractText(BinaryDocumentStream documentStream);

        /// <summary>
        /// Is this text extractor able to handle this format?
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        bool CanHandle(string contentType);
    }
}