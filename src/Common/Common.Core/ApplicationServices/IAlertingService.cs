namespace Bifrost.Common.Core.ApplicationServices
{
    public interface IAlertingService
    {
        void SendAlert(string recipient, string alertTitle, string alertBody);
    }
}
