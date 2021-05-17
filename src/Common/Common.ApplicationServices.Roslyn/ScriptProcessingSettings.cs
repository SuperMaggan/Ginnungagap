using Bifrost.Common.Core.Settings;

namespace Bifrost.Common.ApplicationServices.Roslyn
{
    public class ScriptProcessingSettings : ConfigSettingsBase
    {
        /// <summary>
        /// If a document's script is missing (no domain script), generate the default
        /// Otherwise throw exception
        /// </summary>
        public bool GenerateDefaultScript { get; set; }
    }
}