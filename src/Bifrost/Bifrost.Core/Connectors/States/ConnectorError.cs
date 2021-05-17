using System;

namespace Bifrost.Core.Connectors.States
{
    public class ConnectorError
    {
        public string Message { get; set; }
        public string StackTrace { get;set; }
        public DateTime Date { get; set; }
        public int ErrorCount { get; set; }

    }
}