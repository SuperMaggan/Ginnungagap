//using System;
//using System.Net;
//using Bifrost.Connector.Web.Domain;
//using Bifrost.Connector.Web.Interfaces;
//using HtmlAgilityPack;
//using OpenQA.Selenium.PhantomJS;

//namespace Bifrost.Connector.Web.Implementations.WebPageHandlers
//{
//    public class SeleniumWebPageHandler : IUriHandler
//    {
//        public Page Download(Uri uri)
//        {
//            if (uri.Scheme == "file")
//                return LoadFromDisk(uri);
//            return LoadFromNet(uri);
//        }

//        public Page Download(Uri url, NetworkCredential credential)
//        {
//            throw new NotImplementedException();
//        }

//        public bool CanHandle(Uri url)
//        {
//            return false;
//        }

//        private Page LoadFromNet(Uri uri)
//        {
//            return new WebPage(uri, GetHtmlDocument(uri.AbsoluteUri));
//        }
//        private Page LoadFromDisk(Uri uri)
//        {
//            return new WebPage(uri, GetHtmlDocument(uri.AbsolutePath));
//        }

//        private HtmlDocument GetHtmlDocument(string url)
//        {
//            using (var driver = new PhantomJSDriver())
//            {
//                driver.Navigate().GoToUrl(url);
//                var htlmDoc = new HtmlDocument();
//                htlmDoc.LoadHtml(driver.PageSource);
//                return htlmDoc;
//            }
//        }
//    }

//}