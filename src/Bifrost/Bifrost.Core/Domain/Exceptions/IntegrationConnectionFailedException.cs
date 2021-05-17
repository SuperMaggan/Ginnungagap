using System;

namespace Bifrost.Core.Domain.Exceptions
{
    /// <summary>
    /// Is thrown when a connection to a integration point fails
    /// </summary>
    public class IntegrationConnectionFailedException : Exception
    {
        public IntegrationConnectionFailedException(string message): base(message)
        {
        }
    }
}
