using Bifrost.Core.Settings;
using Bifrost.RestClient.Internal;

namespace Bifrost.IntegrationTests.RestClient
{
    public class AutoResolveScyllaRestClientSettings : AsgardSettingsBase, IAsgardRestClientSettings
    {
        public string AsgardApiUrl { get; set; }
        public int AsgardTimeoutMS { get; set; }
        public bool UseDefaultProxy { get; set; }
        public string AsgardClientApiKey { get; set; }
    }
}