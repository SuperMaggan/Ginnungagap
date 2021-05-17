using Bifrost.Common.Core.ApplicationServices.Helpers;
using Bifrost.Common.Core.Domain.Jobs;
using Bifrost.Common.Tests.TestDomain.Jobs;
using FluentAssertions;
using Xunit;

namespace Bifrost.Common.Tests.ConfigTests
{
    public class ConfigurableJobTests
    {


        [Fact]
        public void CanSerializeSimpleConfig()
        {
            var exampleConfig = new SimpleConfig().CreateExample();
            var mapper = new DefaultJobConfigurationMapper(new IJobConfiguration[] { new SimpleConfig(), new SimpleListConfig() });
            var serialized = mapper.Serialize(exampleConfig);
            var deserialized = mapper.Deserialize(serialized);

            exampleConfig.Should().Be(deserialized);
            

        }

        [Fact]
        public void CanSerializeSimpleListConfig()
        {
            var exampleConfig = new SimpleListConfig().CreateExample();

            var mapper = new DefaultJobConfigurationMapper(new IJobConfiguration[] { new SimpleConfig(), new SimpleListConfig() });
            var serialized = mapper.Serialize(exampleConfig);
            var deserialized = mapper.Deserialize(serialized);
            exampleConfig.Should().Be(deserialized);


        }
        //[TestMethod]
        //public void CanSerializeInterfaceConfig()
        //{
        //    var exampleConfig = new ContainsInterfaceConfig().CreateExample();

        //    var serialized = exampleConfig.Serialize();
        //    var deserialized = exampleConfig.Deserialize(serialized);

        //    exampleConfig.ShouldBeEquivalentTo(deserialized);


        //}
    }
}
