//using System.Collections.Generic;
//using System.Linq;
//using Bifrost.Connector.Web.Interfaces;
//using HtmlAgilityPack;

//namespace Bifrost.Connector.Web.Implementations.LinkScrapers
//{
//    public class FrameScraper : ILinkScraper
//    {
//        public IList<string> ScrapeLinks(HtmlDocument htmlDocument)
//        {
//            var frames = htmlDocument.DocumentNode.SelectNodes("//frame");
//            if (frames== null)
//                return new List<string>();
//            var srcs= frames.Select(x =>
//                                        {
//                                            var src = x.Attributes["src"];
//                                            if (src != null)
//                                                return x.Attributes["src"].Value;
//                                            return "";
//                                        });

//            var foundLinks = srcs.Distinct()
//                .Where(IsValidUrl)
//                .ToList();
//            return foundLinks;
//        }

//        public bool CanHandle(HtmlDocument htmlDocument)
//        {
//            return true;
//        }


//        private bool IsValidUrl(string url)
//        {
//            if (url.Contains("#"))
//                return false;

//            return true;
//        }
//    }
//}