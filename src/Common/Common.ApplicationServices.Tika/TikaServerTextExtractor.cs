using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain;
using Bifrost.Common.Core.Domain.TextExtraction;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bifrost.Common.ApplicationServices.Tika
{
    public class TikaServerTextExtractor : ITextExtractor
    {
        private readonly TikaSettings _settings;
        private readonly TimeSpan _timeout;

        public TikaServerTextExtractor(TikaSettings settings)
        {
            _settings = settings;
            _settings.TikaServerUrl.ConnectionString.TrimEnd('/');
            _timeout = new TimeSpan(0,0,0,0,_settings.TikaTimeoutMs);
        }

        public IDocument ExtractText(BinaryDocumentFile binaryDocumentFile)
        {
            using (HttpClient client = new HttpClient()) {
                try
                {
                    using (var fileStream = File.OpenRead(binaryDocumentFile.FilePath))
                    {
                        if (!File.Exists(binaryDocumentFile.FilePath))
                            client.Timeout = _timeout;
                        var requestUrl = $"{_settings.TikaServerUrl.ConnectionString}/rmeta/text";
                        var result = client.PutAsync(requestUrl, new StreamContent(fileStream))
                            .Result;

                        if (!result.IsSuccessStatusCode)
                            throw new TextExtractionException(binaryDocumentFile.Id, result.ReasonPhrase);

                        return ParseTikaResponse(result.Content, binaryDocumentFile.Id, binaryDocumentFile.Domain);
                    }
                }
                catch (Exception e)
                {
                    throw new TextExtractionException(binaryDocumentFile.Id, e.Message, e);
                }
            }
        }

        public IDocument ExtractText(BinaryDocumentStream documentStream)
        {
            
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = _timeout;
                var requestUrl = $"{_settings.TikaServerUrl.ConnectionString}/rmeta/text";
                try
                {
                    var result = client.PutAsync(requestUrl, new StreamContent(documentStream.Stream))
                        .Result;

                    if (!result.IsSuccessStatusCode)
                        throw new TextExtractionException(documentStream.Id, result.ReasonPhrase);

                    return ParseTikaResponse(result.Content, documentStream.Id, documentStream.Domain);
                }
                catch (AggregateException e)
                {
                    throw new TextExtractionException(documentStream.Id, $"Request Timeout of {_timeout.TotalMilliseconds}ms has been exceeded", e);
                }
            }
            

        }

        private IDocument ParseTikaResponse(HttpContent resultContent, string documentId, string domain)
        {
            var textContent = resultContent.ReadAsStringAsync().Result;
            var jContent = JToken.Parse(textContent);
            

        

            return new AddDocument()
            {
                Id = documentId,
                Fields = jContent.Children<JObject>()
                    .SelectMany(jDocument => 
                        jDocument.Properties().Select(prop => new Field(prop.Name, prop.Value.ToString())))
                    .ToList(),
                Domain = domain
            };

        }

        public bool CanHandle(string contentType)
        {
            return true;
        }
    }
}