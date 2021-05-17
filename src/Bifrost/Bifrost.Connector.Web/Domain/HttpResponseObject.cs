using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Bifrost.Core.Connectors.Configs;
using Bifrost.Core.Connectors.Configs.Web;

namespace Bifrost.Connector.Web.Domain
{
    public class  HttpResponseObject
    {
        public HttpContentHeaders Headers { get; set; }
        public byte[] Response { get; set; }
        public string ContentType { get; set; }

        public HttpStatusCode StatusCode { get; set; }
        public Encoding Encoding { get; set; }
        
        public Exception ExceptionThrown { get; set; }

        public WebConnectorJobConfiguration ConfigUsed { get; set; }

        public Uri RequestUrl { get; set; }


        public override string ToString()
        {
            return $"HttpResponseObject: {RequestUrl} (status code: {StatusCode}, ContentType: {ContentType}, encoding: {Encoding})";
        }
    }
}