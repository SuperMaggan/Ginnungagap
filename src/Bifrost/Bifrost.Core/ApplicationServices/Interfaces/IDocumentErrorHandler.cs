using System;
using Bifrost.Common.Core.Domain;
using Bifrost.Core.Domain.Document;
using Bifrost.Common.Core.Domain.TextExtraction;

namespace Bifrost.Core.ApplicationServices.Interfaces
{
    /// <summary>
    /// Handles document that for some reason resultet in an error i.e. in the extraction phase
    /// </summary>
    public interface IDocumentErrorHandler
    {
        void PersistErrorInformation(BinaryDocumentStream stream, TextExtractionException exception);
        void PersistErrorInformation(BinaryDocumentFile file, TextExtractionException exception);
        void PersistErrorInformation(IDocument document, Exception exception);



    }
}