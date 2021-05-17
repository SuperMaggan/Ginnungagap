using Bifrost.Connector.Web.ApplicationServices.Interfaces;
using Bifrost.Connector.Web.Domain.Pages;
using Bifrost.Core.Domain.Document;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Connector.Web.ApplicationServices.Implementations.PageConverters
{
    public class WebPageConverter : IPageConverter
    {
        public IDocument ConvertToDocument(Page page, string category)
        {
            var webPage = page as WebPage;
            var doc = new AddDocument(){Id = page.Id, Domain = category};
            doc.Fields.Add(new Field("Url", page.Url.AbsoluteUri));
            doc.Fields.Add(new Field("Domain", category));
            doc.Fields.Add(new Field("Host", page.Url.Host));
            doc.Fields.Add(new Field("Depth", page.Depth.ToString()));
            doc.Fields.Add(new Field("Html", webPage.Html));
            //foreach (var field in webPage.Metadata)
            //{
            //    field.Name = $"meta_{field.Name}";
            //    doc.Fields.Add(field);
            //}
            //doc.Fields.Add(new Field("Body",webPage.Body));

            return doc;
        }

        public bool CanHandle(Page page)
        {
            return page is WebPage;
        }
    }
}
