using System;
using System.Collections.Generic;
using Bifrost.Common.ApplicationServices.Sql;
using Bifrost.Common.ApplicationServices.Sql.Common;
using Bifrost.Common.ApplicationServices.Sql.Databases;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain;
using FluentAssertions;
using Xunit;

namespace Bifrost.Common.IntegrationTests.DatabaseTests.SqlServer
{
    public class SqlProcessingInformationServiceTests
    {
        public SqlProcessingInformationServiceTests()
        {
            AssertionOptions.AssertEquivalencyUsing(options =>
            {
                options.Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation)).WhenTypeIs<DateTime>();
                options.Using<DateTimeOffset>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation)).WhenTypeIs<DateTimeOffset>();
                return options;
            });
        }
        private IProcessInformationService CreateService()
        {
            var settings = new CommonDatabaseSettings();
            var database = new ProcessInformationDatabase(settings);

            return new SqlProcessInformationService(new IProcessInformationCollector[0], database);
        }



        [Fact]
        public void Can_create_snapshot()
        {

            var service = CreateService();
            var info = service.CreateSnapshot("JobsRunner");
            info.Should().NotBeNull();
        }


        [Fact]
        public void Can_save_current()
        {

            var service = CreateService();
            service.SaveCurrent("JobsRunner");
            
        }


        [Fact]
        public void Can_save_and_load_current()
        {

            var service = CreateService();
            var current = service.CreateSnapshot("JobsRunner");
            service.Save(current);
            var actualCurrent = service.Get(current.Id);

            current.Should().Be(actualCurrent);

        }
    }
}