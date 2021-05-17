namespace Bifrost.Common.Core.Domain
{
    /// <summary>
    /// This represent that this document has been deleted
    /// </summary>
    public class DeleteDocument : IDocument
    {
        /// <summary>
        /// Id of the document to be deleted
        /// </summary>
        public string Id { get; set; }
        public string Domain { get; set; }
        public DeleteDocument(string id, string domain)
        {
            Id = id;
            Domain = domain;
        }
        public IDocument Clone()
        {

            return new DeleteDocument(this.Id, this.Domain);
        }
    }
}