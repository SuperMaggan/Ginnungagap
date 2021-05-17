using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Domain.Document;
using Bifrost.Core.Settings;
using Bifrost.Core.Utils;
using Bifrost.Common.Core.Domain;
using Serilog;

namespace Bifrost.Core.ApplicationServices.Implementations
{
    public class FileSystemIntegrationService : IIntegrationService
    {
        protected readonly string BaseDirectory;
        protected readonly string AddDirectory;
        protected readonly string DeleteDirectory;


        public FileSystemIntegrationService(CommonSettings settings)
            : this(settings.FileSystemDirectory)
        {

        }

        public FileSystemIntegrationService(string baseDirectoryPath)
        {
            BaseDirectory = baseDirectoryPath;
            AddDirectory = Path.Combine(BaseDirectory, "Adds");
            DeleteDirectory = Path.Combine(BaseDirectory, "Deletes");
            VerifyAndCreateDirectories();
        }

        private void VerifyAndCreateDirectories()
        {
            if (!Directory.Exists(BaseDirectory))
                Directory.CreateDirectory(BaseDirectory);
            if (!Directory.Exists(AddDirectory))
                Directory.CreateDirectory(AddDirectory);
            if (!Directory.Exists(DeleteDirectory))
                Directory.CreateDirectory(DeleteDirectory);
        }

        public bool CanConnect()
        {
            VerifyAndCreateDirectories();
            return true;
        }

        public void HandleDocuments(IList<IDocument> documents)
        {
            string domain = documents.FirstOrDefault()?.Domain;
            if(documents.Select(x=>x.Domain).Distinct().Count() > 1)
                throw new NotSupportedException("No support for calling HandleDocuments() with documents of different domains.");
            AddDocuments(documents.Where(d => d is AddDocument).Cast<AddDocument>(),domain);
            DeleteDocuments(documents.Where(d => d is DeleteDocument).Cast<DeleteDocument>(), domain);
        }

        private void AddDocuments(IEnumerable<AddDocument> documents, string domain)
        {
            string domainDirectory = Path.Combine(AddDirectory, domain);
            if (!Directory.Exists(domainDirectory))
                Directory.CreateDirectory(domainDirectory);

            var xRootDoc = new XDocument();
            var root = new XElement("Documents");

            foreach (var document in documents)
            {
                var xDocument = new XElement("Document");
                xDocument.Add(new XAttribute("Id", document.Id));
                xDocument.Add(new XAttribute("Domain", document.Domain));
                foreach (var field in document.Fields)
                {
                    if (field == null)
                        continue;

                    xDocument.Add(new XElement(XmlUtils.ReplaceInvalidElementNameChars(field.Name),
                        XmlUtils.ReplaceHexadecimalSymbols(field.Value)));
                }
                root.Add(xDocument);
            }
            xRootDoc.Add(root);
            var path = Path.Combine(domainDirectory, Guid.NewGuid() + ".xml");
            using (var textWriter = File.CreateText(path))
            {
                root.Save(textWriter, SaveOptions.None);
            }
            

        }


        private void DeleteDocuments(IEnumerable<DeleteDocument> delDocuments, string domain)
        {
            int maxBatchSize = 200;
            string domainDirectory = Path.Combine(DeleteDirectory, domain);
            if (!Directory.Exists(domainDirectory))
                Directory.CreateDirectory(domainDirectory);

            var batches = delDocuments.InBatches(maxBatchSize);
            foreach (var batch in batches)
            {
                SerializeDeletes(batch.ToList(), domainDirectory);
            }
            
        }

        private void SerializeDeletes(IList<DeleteDocument> deleteDocuments, string directory)
        {
            var root = new XElement("Deletes");
            foreach (var delDoc in deleteDocuments)
            {
                var xDocument = new XElement("Delete");
                xDocument.Add(new XAttribute("Id", delDoc.Id));
                xDocument.Add(new XAttribute("Domain", delDoc.Domain));
                root.Add(xDocument);
            }
            using (var textWriter = File.CreateText(Path.Combine(directory, Guid.NewGuid() + ".xml")))
            {
                root.Save(textWriter, SaveOptions.None);
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
            return $"{this.GetType().Name}: CanConnect() to {BaseDirectory}: {CanConnect()}";
        }
    }
}