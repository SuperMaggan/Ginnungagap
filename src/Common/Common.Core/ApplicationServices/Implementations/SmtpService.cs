

using System;

namespace Bifrost.Common.Core.ApplicationServices.Implementations
{
    /// <summary>
    /// Sends alerts as emails using service-wide SMTP settings 
    /// defined in the system.net/mailSettings part of the config file
    /// </summary>
    public class SmtpService : IAlertingService
    {
        public void SendAlert(string recipient, string alertTitle, string alertBody)
        {
            throw new NotSupportedException("Coming in netstandard 2.0");
            //using (var client = new SmtpClient())
            //{
            //    var message = new MailMessage
            //    {
            //        Body = alertBody,
            //        Subject = alertTitle
            //    };
            //    message.To.Add(recipient);
            //    client.Send(message);   
            //}
        }
    }
}
