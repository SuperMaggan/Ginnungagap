namespace Bifrost.Core.Settings
{
    public class CommonSettings : AsgardSettingsBase
    {

        /// <summary>
        /// Root directory where to store the states
        /// </summary>
        public string StateRootDirectory { get; set; } = "State";


        /// <summary>
        /// Serialize documents to XML files on disk
        /// </summary>
        public bool UseFileSystemIntegration { get; set; } = true;

        /// <summary>
        /// Where to store the files
        /// </summary>
        public string FileSystemDirectory { get; set; } = "Data";

        /// <summary>
        /// If the file system should be used if one of the integration point's not responding
        /// When the integration point start responding again the saved documents
        /// will be retried
        /// </summary>
        public bool UseFileSystemAsFailover { get; set; } = true;
        
        public bool UseNullIntegration { get; set; }


        /// <summary>
        /// When serializing DateTime to string, use this format
        /// If empty, then invariant culture will be used
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// Maximum number of characters (UTF8: 1 char is 4 bytes) a field should contain. Any more will be truncated
        /// Defaults to 10000kB
        /// </summary>
        public int MaxFieldSizeKb { get; set; } = 10000;


    }
}
