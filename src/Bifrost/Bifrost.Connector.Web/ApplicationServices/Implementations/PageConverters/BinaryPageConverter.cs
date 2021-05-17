using System.IO;
using System.Linq;
using Bifrost.Connector.Web.ApplicationServices.Interfaces;
using Bifrost.Connector.Web.Domain.Pages;
using Bifrost.Core.Domain.Document;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain;
using Bifrost.Common.Core.Domain.TextExtraction;
using Serilog;

namespace Bifrost.Connector.Web.ApplicationServices.Implementations.PageConverters
{
    public class BinaryPageConverter : IPageConverter
    {
        private readonly ITextExtractionService _textExtractorService;

        public BinaryPageConverter(ITextExtractionService textExtractorService)
        {
            _textExtractorService = textExtractorService;
        }

        public IDocument ConvertToDocument(Page page, string domain)
        {

            var doc = new AddDocument(){Id = page.Id , Domain = domain};
            doc.Fields.Add(new Field("Url", page.Url.AbsoluteUri));
            doc.Fields.Add(new Field("Domain", domain));
            doc.Fields.Add(new Field("Depth", page.Depth.ToString()));
            doc.Fields.Add(new Field("Host", page.Url.Host));
            using (var memoryStream = new MemoryStream((page as BinaryPage).Data))
            {
                var extractedDoc =
                    _textExtractorService.ExtractText(new BinaryDocumentStream(page.Url.ToString(), domain, memoryStream));

                if (extractedDoc.TextExtractionException != null)
                {
                    doc.Fields.Add(new Field("ErrorMessage", extractedDoc.TextExtractionException.Message));
                    doc.Fields.Add(new Field("ErrorStackTrace", extractedDoc.TextExtractionException.StackTrace));
                }
                if (extractedDoc.ResultingDocument == null)
                    return doc;
                foreach (var field in (extractedDoc.ResultingDocument as AddDocument).Fields)
                {
                    doc.Fields.Add(field);
                }
            }
            return doc;
        }

        public bool CanHandle(Page page)
        {
            return page is BinaryPage;
        }
    }
}