using Bifrost.Common.Core.Settings;

namespace Bifrost.Common.ApplicationServices.Tika
{
    public class TikaSettings : ConfigSettingsBase
    {
        /// <summary>
        /// Time in milliseconds before an exception will be thrown when extracting a document
        /// </summary>
        public int TikaTimeoutMs { get; set; } = 4000;

        /// <summary>
        /// Url to Tika server endpoint, eg. http://localhost:9998
        /// </summary>
        public ConnectionStringSettings TikaServerUrl { get; set; }
    }
}
