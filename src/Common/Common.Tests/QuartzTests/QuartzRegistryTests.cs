using Bifrost.Common.QuartzIntegration.Infrastructure;
using StructureMap;
using Xunit;

namespace Bifrost.Common.Tests.QuartzTests
{
    public class QuartzRegistryTests
    {
        [Fact]
        public void Can_setup_quartzserver()
        {
            var container = new Container(new CommonQuartzModule());
            container.AssertConfigurationIsValid();
        }

    }
}