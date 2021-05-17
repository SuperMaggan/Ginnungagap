using Bifrost.Core.Domain.Document;
using Bifrost.Common.Core.ApplicationServices;

namespace Bifrost.Processing.CsScript
{
    public interface IScript
    {

        /// <summary>
        /// Called when processing an ADD document
        /// </summary>
        /// <param name="initialDocument"></param>
        /// <param name="textExtractorService"></param>
        /// <returns></returns>
        DocumentBase ProcessDocument(AddDocument initialDocument, ITextExtractionService textExtractorService);

        /// <summary>
        /// CAlled when processing an DELETE document
        /// </summary>
        /// <param name="initialDocument"></param>
        /// <returns></returns>
        DocumentBase ProcessDelete(DeleteDocument initialDocument);
    }
}
