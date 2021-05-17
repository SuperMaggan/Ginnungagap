namespace Bifrost.Connector.Web.Domain.Pages
{
    public class NotAuthorizedPage : Page
    {
        public NotAuthorizedPage(string id) : base(id)
        {
        }

        public string Reason { get; set; }
        public int Status { get; set; }
        protected override string CalculateHash()
        {
            return "";
        }
    }
}