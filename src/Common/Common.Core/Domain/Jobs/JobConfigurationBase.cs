namespace Bifrost.Common.Core.Domain.Jobs
{
    public abstract class JobConfigurationBase : IJobConfiguration
    {
        /// <summary>
        /// FullName of the implementing type. Identifier for this type of job 
        /// </summary>
        public string TypeName => this.GetType().FullName;

        /// <summary>
        /// Describing what kind of job this configuration is for. What does it do?
        /// </summary>
        public abstract string Description { get; }


        /// <summary>
        /// Creates an example of this configuration type
        /// </summary>
        /// <returns></returns>
        public abstract IJobConfiguration CreateExample();

        /// <summary>
        /// Display name for this type of name
        /// </summary>
        public abstract string DisplayName { get; }

        /// <summary>
        /// Name of the job instance running this 
        /// Should be set by the executing job
        /// </summary>
        //[XmlIgnore]
        public string JobName { get; set; }
    }
}