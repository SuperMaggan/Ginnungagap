using Bifrost.Connector.Web.Domain.Pages;
using Bifrost.Core.Domain.Document;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Connector.Web.ApplicationServices.Interfaces
{
    public interface IPageConverter
    {
        IDocument ConvertToDocument(Page page, string domain);
        bool CanHandle(Page page);
    }
}