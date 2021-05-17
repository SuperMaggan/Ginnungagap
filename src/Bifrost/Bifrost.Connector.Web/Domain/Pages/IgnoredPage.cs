namespace Bifrost.Connector.Web.Domain.Pages
{
    public class IgnoredPage : Page
    {
        public IgnoredPage(string id, string ignoreReason) : base(id)
        {
            Reason = ignoreReason;
        }
        public string Reason { get; set; }
        protected override string CalculateHash()
        {
            return "";
        }
        
    }
}