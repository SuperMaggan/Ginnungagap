using Bifrost.Common.Core.Domain.Jobs;

namespace Bifrost.Common.IntegrationTests.QuartzIntegration.TestJobs
{
    internal class TestJobConfiguration : JobConfigurationBase
    {
        public string Message { get; set; }

        /// <summary>
        /// Describing what kind of job this configuration is for. What does it do?
        /// </summary>
        public override string Description { get; }

        public override IJobConfiguration CreateExample()
        {
            return new TestJobConfiguration()
            {
                Message = "I am job"
            };
        }

        /// <summary>
        /// Human readable name of this configuration type
        /// </summary>
        public override string DisplayName => "Test job";
    }
}