namespace Bifrost.Common.Core.Domain.TextExtraction
{
    /// <summary>
    /// represents a file stored on the file system
    /// </summary>
    public class BinaryDocumentFile
    {
        public BinaryDocumentFile(string id, string domain, string filePath)
        {
            Id = id;
            Domain = domain;
            FilePath = filePath;
        }

        public string Id { get; set; }
        public string Domain {get; set; }

        public string FilePath { get; set; }
    }
}
