using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using Bifrost.Core.Connectors.Configs;
using Bifrost.Core.Connectors.Configs.Web;

namespace Bifrost.Connector.Web.ApplicationServices.Implementations.PageHandlers
{
    public abstract class PageHandlerBase
    {
        protected string GetPageId(Uri url, HttpContentHeaders headers = null)
        {
            //Example: Link:</node/6>; rel="shortlink",</home>; rel="canonical"
            var linkHead = "";
            if (headers.TryGetValues("Link", out IEnumerable<string> result))
                linkHead = result.FirstOrDefault();
                
            if (string.IsNullOrEmpty(linkHead))
                return url.ToString();
            var tokens = linkHead.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var token in tokens)
            {
                var subTokens = token.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (subTokens.Length != 2)
                    continue;
                if (subTokens[1].Contains("canonical"))
                    return subTokens[0].Trim('<', '>');
            }
            return url.ToString();
        }

        protected DateTime GetVerifyDate(HttpContentHeaders headers,WebConnectorJobConfiguration config )
        {
            //Example: Expires:Sun, 19 Nov 1978 05:00:00 GMT
            var expiresHead = headers.Expires;
            
            //If no value or value that is less than today
            if (!expiresHead.HasValue || expiresHead.Value.DateTime < DateTime.UtcNow.AddDays(-1))
            {
                return DateTime.UtcNow + config.DefaultVerifyFrequency.ToTimeSpan();
            }

            return expiresHead.Value.DateTime;
        }

        protected bool StatusCodeIsGreen(HttpStatusCode code)
        {
            return (int)code >= 200 && (int)code < 300;
        }

    }
}