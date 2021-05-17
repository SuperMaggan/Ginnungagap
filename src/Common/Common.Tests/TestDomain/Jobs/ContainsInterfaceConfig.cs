
using Bifrost.Common.Core.Domain.Jobs;

namespace Bifrost.Common.Tests.TestDomain.Jobs
{
    public class ContainsInterfaceConfig : JobConfigurationBase
    {
        public ISimple Simple{ get; set; }
        public int Number { get; set; }


        /// <summary>
        /// Describing what kind of job this configuration is for. What does it do?
        /// </summary>
        public override string Description { get; }

        public override IJobConfiguration CreateExample()
        {
            return new ContainsInterfaceConfig()
            {
                Simple =  new SimpleTon()
                {
                    Name = "Work work"
                },
                Number = 4
            };
        }

        /// <summary>
        /// Human readable name of this configuration type
        /// </summary>
        public override string DisplayName => this.GetType().Name;
    }

    public interface ISimple
    {
        
    }

    public class SimpleTon : ISimple
    {
        public string Name { get; set; }

    }
}