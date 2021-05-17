namespace Bifrost.Common.Core.Domain
{
    /// <summary>
    /// This document is ignored for whatever reason
    /// </summary>
    public class IgnoreDocument : IDocument
    {
        public string Id { get; set; }
        public string Domain { get; set; }
        public string IgnoreReason { get; set; }

        public IgnoreDocument(string id, string domain, string ignoreReason)
        {
            Id = id;
            Domain = domain;
            IgnoreReason = ignoreReason;
        }
        
        public IDocument Clone()
        {
            return new IgnoreDocument(this.Id, this.Domain, this.IgnoreReason);
        }
    }
}