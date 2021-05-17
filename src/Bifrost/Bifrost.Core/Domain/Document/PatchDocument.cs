using Bifrost.Common.Core.Domain;

namespace Bifrost.Core.Domain.Document
{
    public class PatchDocument : AddDocument
    {
        public PatchDocument(string id, string domain) : base(id, domain)
        {
        }
    }
}
