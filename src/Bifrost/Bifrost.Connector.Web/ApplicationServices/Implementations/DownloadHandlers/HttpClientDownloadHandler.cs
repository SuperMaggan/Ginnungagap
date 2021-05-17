using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Bifrost.Connector.Web.ApplicationServices.Interfaces;
using Bifrost.Connector.Web.Domain;
using Bifrost.Core.Connectors.Configs;
using Bifrost.Core.Connectors.Configs.Web;
using Serilog;
using Serilog.Core;

namespace Bifrost.Connector.Web.ApplicationServices.Implementations.DownloadHandlers
{
    public class HttpClientDownloadHandler : IDownloadHandler
    {
        private readonly TimeSpan _timeout = new TimeSpan(0,0,0,0,10000);
        public HttpResponseObject Download(Uri url, WebConnectorJobConfiguration config, NetworkCredential credential = null)
        {

            using(var handler = new HttpClientHandler() { Credentials = credential ?? CredentialCache.DefaultCredentials, AllowAutoRedirect = true})
            using (var client = new HttpClient(handler){Timeout = _timeout})
            {
                try
                {
                    var response = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result;
                    if (!response.IsSuccessStatusCode)
                        return new HttpResponseObject()
                        {
                            ConfigUsed = config,
                            RequestUrl = url,
                            StatusCode = response.StatusCode
                        };
                    return CreateResponseObject(response, config);

                }
                catch (Exception e)
                {
                    Log.Logger.Error($"Error when crawling {url}: {e.Message}");
                    throw e;
                }
            }
        }

        private HttpResponseObject CreateResponseObject(HttpResponseMessage response, WebConnectorJobConfiguration config)
        {
            Encoding encoding = GetEncoding(response);
            var responseObject = new HttpResponseObject()
            {
                RequestUrl = response.RequestMessage.RequestUri,
                ConfigUsed = config,
                ContentType = response.Content.Headers.ContentType.MediaType,
                Encoding = encoding,
                Headers = response.Content.Headers,
                StatusCode = response.StatusCode,
            };

            using (var sr = response.Content.ReadAsStreamAsync().Result)
            {
                responseObject.Response = ReadFully(sr, response.Content.Headers.ContentLength);
            }


            return responseObject;
        }

        private static Encoding GetEncoding(HttpResponseMessage response)
        {
            if (string.IsNullOrEmpty(response.Content.Headers.ContentType?.CharSet))
                return Encoding.UTF8;
            try
            {
                return Encoding.GetEncoding(response.Content.Headers.ContentType.CharSet);
            }
            catch (Exception)
            {
                return Encoding.UTF8;
            }

        }

        private byte[] ReadFully(Stream stream, long? headerContentLength)
        {
            if (!headerContentLength.HasValue || headerContentLength.Value <= 0)
                headerContentLength = 32768;
            byte[] buffer = new byte[headerContentLength.Value];
            using (var ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }
        

    }
}
