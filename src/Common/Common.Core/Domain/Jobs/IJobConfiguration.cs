
namespace Bifrost.Common.Core.Domain.Jobs
{
    public interface IJobConfiguration
    {
        /// <summary>
        /// Human readable name of this configuration type
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// FullName of the implementing type
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// Describing what kind of job this configuration is for. What does it do?
        /// </summary>
        string Description { get; }

        IJobConfiguration CreateExample();
        
        
        string JobName { get; set; }
    }
}