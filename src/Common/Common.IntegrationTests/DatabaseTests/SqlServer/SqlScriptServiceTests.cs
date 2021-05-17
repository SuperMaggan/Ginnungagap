using System;
using Bifrost.Common.ApplicationServices.Sql;
using Bifrost.Common.ApplicationServices.Sql.Common;
using Bifrost.Common.ApplicationServices.Sql.Databases;
using Bifrost.Common.Core.Domain;
using FluentAssertions;
using Xunit;

namespace Bifrost.Common.IntegrationTests.DatabaseTests.SqlServer
{
    public class SqlScriptServiceTests : IDisposable
    {

        public SqlScriptServiceTests()
        {
            var settings = new CommonDatabaseSettings();
            _database = new ScriptDatabase(settings);
            AssertionOptions.AssertEquivalencyUsing(options =>
            {
                options.Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation)).WhenTypeIs<DateTime>();
                options.Using<DateTimeOffset>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation)).WhenTypeIs<DateTimeOffset>();
                return options;
            });
        }
        public void Dispose()
        {
            _database?.Destroy();
        }

        private readonly ScriptDatabase _database;
        SqlScriptRepository CreateService()
        {
           
            return new SqlScriptRepository(_database);
        }



        
        [Fact]
        public void CanCRUD()
        {
                var service = CreateService();

                var script = new Script
                {
                    Name = "ABCD",
                    DomainName = "TestDomain",
                    Code = "//noop",
                    LastUpdated = DateTime.Today,
                    Metadata = "Im green",
                    IsPublic = false,
                    Owner = "Magnus"
                    
                };

                service.SaveOrUpdate(script);
                var fetchedScript = service.GetScriptByName(script.Name);
                fetchedScript.Should().Be(script);

                var updatedScript = new Script
                {
                    Name = script.Name,
                    DomainName = script.DomainName,
                    Code = "// still a noop",
                    LastUpdated = DateTime.Today
                
                };
                service.SaveOrUpdate(updatedScript);
                service.GetScriptByName(updatedScript.Name).Should().Be(updatedScript);

                service.Delete(updatedScript.Name);
                service.GetScriptByName(updatedScript.Name).Should().BeNull();
          
        }

        [Fact]
        public void CanCRUDByDomainName()
        {
            var service = CreateService();

            var script = new Script
            {
                Name = "ABCD",
                DomainName = "TestDomain",
                Code = "//noop",
                LastUpdated = DateTime.UtcNow,
                Metadata = "Im blue"
            };

            service.SaveOrUpdate(script);
            service.GetScriptByDomainName(script.DomainName).Should().Be(script);

            var updatedScript = new Script
            {
                Name = script.Name,
                DomainName = script.DomainName,
                Code = "// still a noop",
                LastUpdated = DateTime.UtcNow,
                Owner = "Maggan"
            };
            service.SaveOrUpdate(updatedScript);
            service.GetScriptByDomainName(script.DomainName).Should().Be(updatedScript);

            service.Delete(updatedScript.Name);
            service.GetScriptByDomainName(updatedScript.DomainName).Should().BeNull();
        }

      
    }
}