using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Domain.Document;
using Bifrost.Core.Settings;
using Bifrost.Common.Core.Domain;
using Serilog;

namespace Bifrost.Core.ApplicationServices.Implementations
{
    public class FileSystemDocumentCache : FileSystemIntegrationService, IDocumentCache
    {
        public FileSystemDocumentCache(CommonSettings settings)
            : base(Path.Combine(settings.FileSystemDirectory, "Cache"))
        {
        }

        /// <summary>
        /// Loads the first batch of documents stored in the cache
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="removeLoadedDocuments"></param>
        /// <returns></returns>
        public IEnumerable<AddDocument> LoadDocumentsBatch(string domain, bool removeLoadedDocuments)
        {
            var filePath = DiscoverFiles(AddDirectory, domain).FirstOrDefault();
            if (filePath == null)
                return new List<AddDocument>();

            var documents = ParseDocumentFile(filePath);
            if(removeLoadedDocuments)
                File.Delete(filePath);
            return documents;

        }

        public IEnumerable<DeleteDocument> LoadDeleteBatch(string domain, bool removeLoadedDeletes)
        {
            var filePath = DiscoverFiles(DeleteDirectory, domain).FirstOrDefault();
            if (filePath == null)
                return new List<DeleteDocument>();

            var deleteDocuments = ParseDeleteFile(filePath);
            if (removeLoadedDeletes)
                File.Delete(filePath);
            return deleteDocuments;
        }

        private IEnumerable<AddDocument> ParseDocumentFile(string filePath, string documentRootElementName = "Document")
        {
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
                return new List< AddDocument > ();

            try
            {
                if((fileInfo.Length / 1048576) > 200)
                    throw new Exception(string.Format("The file {0} is too big to safely be parsed ({1}bytes).", fileInfo.FullName, fileInfo.Length));
                using (var reader = fileInfo.OpenRead())
                {
                    var xDoc = XDocument.Load(reader);
                    var docElements = xDoc.Descendants(documentRootElementName);
                    //if (!docElements.Any())
                    //{
                    //    Logger.WarnFormat("{0} contained no {1} elements. Skipping.", fileInfo, documentRootElementName);
                    //    return new List<AddDocument>();
                    //}
                    
                    return docElements.Select(ParseDocumentElement)
                        .Where(parsedDoc => parsedDoc != null);
                }
            }
            catch (Exception e)
            {
                var failDir = Path.Combine(BaseDirectory, "ParseErrors",fileInfo.Directory.Name);
                if (!Directory.Exists(failDir))
                    Directory.CreateDirectory(failDir);
                
                Log.Error(e, string.Format("Error while parsing file {0}. Moving it to {1}", filePath, failDir));

                
                fileInfo.MoveTo(Path.Combine(failDir,fileInfo.Name));
                throw new Exception(string.Format("Error while parsing file {0}. Moving it to {1}", filePath, failDir), e);
            }
        }

        public IEnumerable<AddDocument> LoadAllDocuments(string domain, bool removeLoadedDocuments)
        {
            var filePaths = DiscoverFiles(AddDirectory,domain).ToList();
            foreach (var filePath in filePaths)
            {
                foreach (var document in ParseDocumentFile(filePath))
                {
                    yield return document;
                }
            }
            if(removeLoadedDocuments)
                foreach (var filePath in filePaths)
                {
                    File.Delete(filePath);
                }
        }


        public IEnumerable<DeleteDocument> LoadAllDeletes(string domain, bool removeLoadedDeletes)
        {
            var filePaths = DiscoverFiles(DeleteDirectory, domain).ToArray();
            foreach (var filePath in filePaths)
            {
                foreach (var deleteDocuments in ParseDeleteFile(filePath))
                {
                    yield return deleteDocuments;
                }
            }
            if (removeLoadedDeletes)
                foreach (var filePath in filePaths)
                {
                    File.Delete(filePath);
                }
        }

        public IEnumerable<string> LoadAllDomainNames()
        {
            var directories = Directory.EnumerateDirectories(AddDirectory)
                .Concat(Directory.EnumerateDirectories(DeleteDirectory));
            return directories
                .Select(Path.GetFileNameWithoutExtension)
                .Distinct();
        }

        /// <summary> 
        /// </summary>
        /// <param name="baseDirectory">Either use the addDirectory or the deleteDirectory</param>
        /// <returns>File paths</returns>
        private IEnumerable<string> DiscoverFiles(string baseDirectory, string domainName)
        {
            var domainDirectoryPath = Path.Combine(baseDirectory, domainName);
            if (!Directory.Exists(domainDirectoryPath))
                yield break;

            foreach (var fileName in Directory.EnumerateFiles(domainDirectoryPath, "*.xml"))
            {
                yield return Path.Combine(domainDirectoryPath, fileName);
            }

        }

        private IEnumerable<DeleteDocument> ParseDeleteFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var foundDeletes = new List<DeleteDocument>();

            if (!fileInfo.Exists)
                return foundDeletes.ToArray();

            try
            {
                using (var reader = fileInfo.OpenRead())
                {
                    var xDoc = XDocument.Load(reader);
                    var deleteElements = xDoc.Descendants("Delete");
                    if (!deleteElements.Any())
                    {
                        Log.Warning($"{fileInfo} contained no DELETE elements. Skipping.");
                        return foundDeletes.ToArray();
                    }
                    foundDeletes
                        .AddRange(deleteElements.Select(ParseDeleteElement)
                                      .Where(deleteField => deleteField != null));
                }
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error while parsing file {filePath} : {e.Message}");
                throw new Exception("Error while parsing file " + filePath, e);
            }
            return foundDeletes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="documentRootElementName">By default Document</param>
        /// <returns></returns>
        private AddDocument ParseDocumentElement(XElement element)
        {
            var id = element.Attribute("Id");
            var domain = element.Attribute("Domain");
            if (id == null || domain == null)
                return null;
            var document = new AddDocument(id.Value, domain.Value);
            var fields = new List<Field>();

            foreach (var fieldElement in element.Descendants())
            {
                var fieldName = fieldElement.Name.LocalName;
                var fieldValue = fieldElement.Value;
                fields.Add(new Field(fieldName, fieldValue));
            }

            document.Fields = fields;
            return document;
        }
        private DeleteDocument ParseDeleteElement(XElement deleteElement)
        {
            var id = deleteElement.Attribute("Id");
            var domain = deleteElement.Attribute("Domain");
            if (id == null || domain == null)
                return null;
            return new DeleteDocument(id.Value, domain.Value);
        }
    }
}