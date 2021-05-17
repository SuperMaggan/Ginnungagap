using System;

namespace Bifrost.Dto.Dto
{
    public class ErrorDto
    {
        
        public string Message { get; set; }
        
        public string StackTrace { get; set; }

        /// <summary>
        /// When this error occurred
        /// </summary>
        public DateTime Date { get; set; }
    }
}