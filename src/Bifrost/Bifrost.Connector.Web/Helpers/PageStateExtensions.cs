using System;
using Bifrost.Connector.Web.Domain;
using Bifrost.Connector.Web.Domain.Pages;
using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Domain.Document;

namespace Bifrost.Connector.Web.Helpers
{
    public static class PageStateExtensions
    {
        private const string Delimiter = "<|>";
        public static void UpdatePageState(this IDocumentStateService service, Page page, string jobName)
        {
            service.SaveOrUpdate(jobName, page.ToDocumentState());
        }

        public static bool ShouldVerify(this PageState pageState)
        {
            return pageState == null || DateTime.UtcNow < pageState.VerifyDate;
        }

        public static PageState ToPageState(this DocumentState documentState)
        {
            if (documentState == null)
                return null;
            var tokens = documentState.OptionalData.Split(new string[] { Delimiter }, StringSplitOptions.None);
            var depth = 0;
            int.TryParse(tokens[0], out depth);
            var url = tokens.Length == 2 ? new Uri(tokens[1]) : new Uri(documentState.DocumentId);
            return new PageState()
            {
                Id = documentState.DocumentId,
                Url = url,
                HashValue = documentState.HashValue,
                LastUpdated = documentState.LastUpdated,
                LastVerified = documentState.LastVerified,
                VerifyDate = documentState.VerifyDate,
                Depth = depth
            };
        }

        public static DocumentState ToDocumentState(this Page page)
        {
            return new DocumentState()
            {
                DocumentId = page.Id,
                HashValue = page.HashValue,
                LastUpdated = page.DownloadDate,
                LastVerified = page.DownloadDate,
                VerifyDate = page.VerifyDate,
                OptionalData = $"{page.Depth}{Delimiter}{page.Url}"
            };
        }

        

        public static bool PageIsChanged(this PageState docState, Page page)
        {
            return docState == null || docState.HashValue != page.HashValue;
        }
        
    }
}