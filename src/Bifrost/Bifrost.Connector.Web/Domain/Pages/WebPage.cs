using System.Collections.Generic;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Connector.Web.Domain.Pages
{
    public class WebPage : Page
    {
        public WebPage(string id) : base(id)
        {
        }

        public IList<Field> Metadata { get; set; }
        public string Body { get; set; }
             
        public string  Html{ get; set; }

        protected override string CalculateHash()
        {
            return CalculateMD5Hash(Html);
        }
    }
}
