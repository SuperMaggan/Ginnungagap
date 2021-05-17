using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Domain.Document;
using Bifrost.Core.Utils;
using Bifrost.Common.Core.Domain;
using Serilog;

namespace Bifrost.Integration.Solr
{
    public class SolrIntegrationService : IIntegrationService
    {
        private readonly SolrSettings _solrSettings;
 
        public SolrIntegrationService(SolrSettings solrSettings)
        {
            _solrSettings = solrSettings;
        }
 
        public bool CanConnect()
        {
            var request = (HttpWebRequest)WebRequest.Create(_solrSettings.SolrPingUrl);
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == (HttpStatusCode)200)
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            var streamReader = new StreamReader(responseStream);
                            var sLine = "";
                            var i = 0;

                            while (sLine != null && !sLine.Contains("<str name=\"status\">"))
                            {
                                sLine = streamReader.ReadLine();
                                if (sLine != null)
                                {
                                    if (sLine.Contains("<str name=\"status\">"))
                                    {
                                        var tagStart = sLine.IndexOf("<str name=\"status\">",
                                                                     System.StringComparison.Ordinal);
                                        var tagEnd = sLine.IndexOf(">", tagStart, System.StringComparison.Ordinal) + 1;
                                        var value = sLine.Substring(tagEnd, 2);

                                        if (value == "OK")
                                            return true;

                                    }
                                }
                                i++;
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Log.Logger.Error($"Can't connect to the Solr instance {request.Address} ({e.Message})", e);
                return false;
            }
            return false;
        }

        public void HandleDocuments(IList<IDocument> documents)
        {
            string domain = documents.FirstOrDefault()?.Domain;
            if (documents.Select(x => x.Domain).Distinct().Count() > 1)
                throw new NotSupportedException("No support for calling HandleDocuments() with documents of different domains.");
            AddDocuments(documents.Where(d => d is AddDocument).Cast<AddDocument>());
            PatchDocuments(documents.Where(d => d is PatchDocument).Cast<PatchDocument>());
            DeleteDocuments(documents.Where(d => d is DeleteDocument).Cast<DeleteDocument>());

        }

        private void DeleteDocuments(IEnumerable<DeleteDocument> deleteDocuments)
        {
            var update = new XElement("update");
            var delete = new XElement("delete");

            foreach (var delDoc in deleteDocuments)
            {
                var doc = new XElement(_solrSettings.IdFieldName, delDoc.Id);
                delete.Add(doc);
            }
            update.Add(delete);

            if(_solrSettings.SolrIncludeCommit)
                update.Add(new XElement("commit"));
            PostXmlDocument(update);

        }

        private void Optimize()
        {
            var updateElement = new XElement("update");
            var optimizeElement = new XElement("optimize");
            updateElement.Add(optimizeElement);
            PostXmlDocument(updateElement);
        }

        private void PatchDocuments(IEnumerable<PatchDocument> documents)
        {
            var update = new XElement("update");
            var patch = new XElement("add");

            foreach (var document in documents)
            {
                var doc = PatchXmlElement(document);
                patch.Add(doc);
            }

            update.Add(patch);

            if (_solrSettings.SolrIncludeCommit)
                update.Add(new XElement("commit"));
            PostXmlDocument(update);

        }


        private void AddDocuments(IEnumerable<AddDocument> documents)
        {
            var update = new XElement("update");
            var add = new XElement("add");
            //add.SetAttributeValue("commitWithin", "15000");

            foreach (var document in documents)
            {
                var doc = AddXmlElement(document);
                add.Add(doc);
            }

            update.Add(add);

            if (_solrSettings.SolrIncludeCommit)
                update.Add(new XElement("commit"));
            PostXmlDocument(update);

        }

        private static XElement PatchXmlElement(PatchDocument document)
        {
            var doc = new XElement("doc");
            var idFieldElement = new XElement("field", document.Id);
            idFieldElement.Add(new XAttribute("name", "id"));
            doc.Add(idFieldElement);

            foreach (var field in document.Fields)
            {
                if (String.IsNullOrWhiteSpace(field.Value)) continue;

                var fieldElement = new XElement("field", XmlUtils.ReplaceHexadecimalSymbols(field.Value));
                fieldElement.Add(new XAttribute("name", XmlUtils.ReplaceInvalidElementNameChars(field.Name)));
                fieldElement.Add(new XAttribute("update", "set"));
                doc.Add(fieldElement);

            }

            return doc;
        }


        private static XElement AddXmlElement(AddDocument document)
        {
            var doc = new XElement("doc");
            var idFieldElement = new XElement("field", document.Id);
            idFieldElement.Add(new XAttribute("name", "id"));
            doc.Add(idFieldElement);
            foreach (var field in document.Fields)
            {
                //Solr does not handle empty fields
                if (String.IsNullOrWhiteSpace(field.Value)) continue;

                var fieldElement = new XElement("field", XmlUtils.ReplaceHexadecimalSymbols(field.Value));
                fieldElement.Add(new XAttribute("name", XmlUtils.ReplaceInvalidElementNameChars(field.Name)));
                fieldElement.Add(new XAttribute("update", "set"));
                doc.Add(fieldElement);
            }

            return doc;
        }


        private void PostXmlDocument(XElement document)
        {
            var wc = new WebClient
            {
                Encoding = Encoding.UTF8,
                
            };
           wc.Headers.Set(HttpRequestHeader.ContentType, "application/xml");
            try
            {
                wc.UploadString(_solrSettings.SolrUpdateUrl, document.ToString());
            }
            catch (WebException e)
            {
                if (e.Response == null)
                {
                    throw e;
                }
                var response = e.Response.GetResponseStream();
                var reader = new StreamReader(response);
                string text = reader.ReadToEnd();
                var match = Regex.Match(text, @"<str name=""msg"">(.*?)</str>");
                var message = text;
                if (match.Success)
                {
                    message = match.Groups[0].Value;
                }
                throw new WebException(message, e);
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}: CanConnect() to {1}: {2}",
                this.GetType().Name,
                _solrSettings.SolrUpdateUrl,
                CanConnect());
        }

    }
}