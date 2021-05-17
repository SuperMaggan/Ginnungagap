//using System;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Mime;
//using System.Text;
//using Bifrost.Connector.Web.ApplicationServices.Interfaces;
//using Bifrost.Connector.Web.Domain;
//using Bifrost.Core.Connectors.Configs.Web;
//using HtmlAgilityPack;

//namespace Bifrost.Connector.Web.ApplicationServices.Implementations.PageHandlers
//{
//    public class AgilityPackUriHandler : IUriHandler
//    {
//        private readonly HtmlWeb _htmlWeb;

//        public AgilityPackUriHandler()
//        {
//            _htmlWeb = new HtmlWeb()
//            {
//                OverrideEncoding = Encoding.Default
//            };
//        }

//        public Page Download(Uri uri,WebConnectorJobConfiguration config, NetworkCredential credential=null)
//        {
//            if (uri.Scheme == "file")
//                return LoadFromDisk(uri);
//            return LoadFromNet(uri, credential);
//        }

//        public bool CanHandle(Uri url)
//        {
//            var unableToHandle = new string[] {"pdf", "xls", "docx"};
//            var extension = GetPrefix(url);
//            return !unableToHandle.Contains(extension);
//        }

//        private WebPage LoadFromNet(Uri url, NetworkCredential credential=null)
//        {
//            HtmlDocument doc = credential != null
//                ? _htmlWeb.Load(url.AbsoluteUri,"GET",new WebProxy(), credential)
//                : _htmlWeb.Load(url.AbsoluteUri);
//            return new WebPage(url.ToString())
//            {
//                Html = doc.ToString(),
//                ContentType = "???"
//            };
//        }
//        private WebPage LoadFromDisk(Uri uri)
//        {
//            var text = File.ReadAllText(uri.AbsolutePath);
//            var doc = new HtmlDocument();
//            doc.LoadHtml(text);
//            return new WebPage(uri.ToString()) {
//                Html = doc.ToString(),
//                ContentType = "???"
//            };
//        }

//        private string GetPrefix(Uri url)
//        {
//            var lastSegment = url.Segments.LastOrDefault();
//            if (lastSegment == null)
//                return "";
//            return Path.GetExtension(lastSegment).TrimStart('.');
            
//        }
//    }
//}