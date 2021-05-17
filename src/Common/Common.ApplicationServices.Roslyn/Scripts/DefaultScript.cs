using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain;
using Bifrost.Common.Core.Domain.DocumentProcessing;

public class [SCRIPTNAME] : IScript
{
    public IDocument ProcessDocument(AddDocument initialDocument, IProcessingUtilService processingUtilService)
    {
        var newdoc = initialDocument.Clone() as AddDocument;
        return newdoc;
    }

    public IDocument ProcessDelete(DeleteDocument deleteDocument, IProcessingUtilService processingUtilService)
    {
        return deleteDocument;
    }
}

