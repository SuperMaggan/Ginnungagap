using System.Net;
using Bifrost.Core.Domain;

namespace Bifrost.Connector.Web.Helpers
{
    public static class CredentialExtensions
    {
        public static NetworkCredential ToNetworkCredential(this Credential credential)
        {
            return credential == null 
                    ? CredentialCache.DefaultNetworkCredentials
                 : new NetworkCredential(credential.Username,credential.Password, credential.Domain);
        }
    }
}
