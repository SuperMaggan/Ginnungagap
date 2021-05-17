namespace Bifrost.Connector.Web.Domain.Pages
{
    public class NotFoundPage : Page
    {
        public NotFoundPage(string id) : base(id)
        {
        }

        public string Reason { get; set; }
        protected override string CalculateHash()
        {
            return "";
        }
    }
}