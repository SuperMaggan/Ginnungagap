using System.Collections.Generic;
using Bifrost.Common.Core.Domain.Jobs;

namespace Bifrost.Common.Tests.TestDomain.Jobs
{
    public class SimpleListConfig : JobConfigurationBase
    {
        public List<string> Actions { get; set; }
        public int Number { get; set; }


        /// <summary>
        /// Describing what kind of job this configuration is for. What does it do?
        /// </summary>
        public override string Description => "Also just a test";

        public override IJobConfiguration CreateExample()
        {
            return new SimpleListConfig()
            {
                Actions = new List<string>() { "Kite","Life"},
                Number = 4
            };
        }

        /// <summary>
        /// Human readable name of this configuration type
        /// </summary>
        public override string DisplayName => this.GetType().Name;
    }
}