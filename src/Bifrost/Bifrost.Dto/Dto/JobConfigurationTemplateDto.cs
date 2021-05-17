namespace Bifrost.Dto.Dto
{
    /// <summary>
    /// Boilerplate for creating a specific job configuration
    /// </summary>
    public class JobConfigurationTemplateDto
    {
        /// <summary>
        /// Showcase of what this kind of config may look like,
        /// The root element decides what kind of configuration type this is
        /// </summary>
        public string ExampleConfiguration { get; set; }

        /// <summary>
        /// Human readable name for this configuration type
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// Descriptive text about this config type
        /// </summary>
        public string Description { get; set; }
    }
}