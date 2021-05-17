
using Bifrost.Common.Core.Domain.Jobs;

namespace Bifrost.Common.Tests.TestDomain.Jobs
{
    public class SimpleConfig : JobConfigurationBase
    {
        public string Action { get; set; }
        public int Number { get; set; }


        /// <summary>
        /// Describing what kind of job this configuration is for. What does it do?
        /// </summary>
        public override string Description => "This is jsut a test";

        public override IJobConfiguration CreateExample()
        {
            return new SimpleConfig()
            {
                Action = "Kite",
                Number = 4
            };
        }

        /// <summary>
        /// Human readable name of this configuration type
        /// </summary>
        public override string DisplayName => this.GetType().Name;
    }
}
