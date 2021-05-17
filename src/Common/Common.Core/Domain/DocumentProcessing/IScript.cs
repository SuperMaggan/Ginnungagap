using Bifrost.Common.Core.ApplicationServices;

namespace Bifrost.Common.Core.Domain.DocumentProcessing
{
    public interface IScript
    {
        IDocument ProcessDocument(AddDocument initialDocument, IProcessingUtilService processingUtilService);
        IDocument ProcessDelete(DeleteDocument initialDocument, IProcessingUtilService processingUtilService);
    }
}