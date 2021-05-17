using System;
using System.Net;

namespace Bifrost.RestClient.Internal
{
    /// <summary>
    /// An exception thrown by The webservice
    /// </summary>
    public class RestException :Exception
    {
        /// <summary>
        /// The actual web response the service gave. This includes the human readable error message as well as 
        /// the stack trace
        /// </summary>
        public string HtmlResponse { get; set; }

        public HttpStatusCode StatusCode { get;set;}
        /// <summary>
        /// Initializes a new instance of RestException with the given parameters
        /// </summary>
        /// <param name="message"></param>
        /// <param name="htmlResponse"> </param>
        /// <param name="webExceptionStatus"></param>
        public RestException(string message, string htmlResponse, HttpStatusCode statusCode) : base(message)
        {
            
            HtmlResponse = htmlResponse;
            StatusCode = statusCode;

        }
    }
}