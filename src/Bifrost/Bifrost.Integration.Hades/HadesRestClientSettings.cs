using Asgard.Asgard.Core.Settings;

namespace Asgard.Integration.Hades
{
    public class HadesRestClientSettings : AsgardSettingsBase, IHadesRestClientSettings
    {
        public string HadesClientApiUrl { get; set; }
        public int HadesTimeoutMS { get; set; }
        public bool UseDefaultProxy { get; set; }
        public string HadesClientApiKey { get; set; }
    }
}