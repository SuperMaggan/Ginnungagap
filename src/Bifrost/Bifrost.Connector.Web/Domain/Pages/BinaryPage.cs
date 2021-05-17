namespace Bifrost.Connector.Web.Domain.Pages
{
    public class BinaryPage : Page
    {
        public BinaryPage(string id) : base(id)
        {
        }

        public byte[] Data { get; set; }

   


        protected override string CalculateHash()
        {
            return CalculateMD5Hash(Data);
        }

    }
}