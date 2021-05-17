using System;

namespace Bifrost.Common.Core.Domain.Jobs
{
    public class JobError
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime Date { get; set; }
        public int ErrorCount { get; set; }

    }
}