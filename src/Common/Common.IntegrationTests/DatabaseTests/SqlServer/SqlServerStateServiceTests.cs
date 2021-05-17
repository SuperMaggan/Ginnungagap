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
    public class SqlServerStateServiceTests
    {
        protected IStateService CreateService()
        {
            var settings = new CommonDatabaseSettings();
            var database = new StateDatabase(settings);

            return new SqlStateService(database);
        }

        

        [Fact]
        public void CanSaveAndDeleteState()
        {
            var service = CreateService();
            var state = new State
            {
                Name = "Test",
                Fields = new List<Field>
                {
                    new Field("Test1", "kite"),
                    new Field("Test2", "4"),
                    new Field("Test3", "life")
                }
            };
            service.SaveState(state);
            service.DeleteState(state.Name);
        }

        [Fact]
        public void CanSaveAndLoadState()
        {
            var service = CreateService();
            var state = new State
            {
                Name = "Test",
                Fields = new List<Field>
                {
                    new Field("Test1", "kite"),
                    new Field("Test2", "4"),
                    new Field("Test3", "life")
                }
            };
            service.SaveState(state);

            var loaded = service.LoadState(state.Name);
            loaded.Should().Be(state);
            service.DeleteState(state.Name);
        }

        [Fact]
        public void CanUpdateStateWithSameFields()
        {
            var service = CreateService();
            var state = new State
            {
                Name = "Test",
                Fields = new List<Field>
                {
                    new Field("Test1", "kite"),
                    new Field("Test2", "4"),
                    new Field("Test3", "life")
                }
            };
            service.SaveState(state);
            state.SetValue("Test3", "Ever");
            service.SaveState(state);

            var loaded = service.LoadState(state.Name);
            loaded.GetValue("Test3").Should().Be("Ever");
            service.DeleteState(state.Name);
        }

        [Fact]
        public void CanUpdateStateWithAddedFields()
        {
            var service = CreateService();
            var state = new State
            {
                Name = "Test",
                Fields = new List<Field>
                {
                    new Field("Test1", "kite"),
                    new Field("Test2", "4"),
                    new Field("Test3", "life")
                }
            };
            service.SaveState(state);
            state.SetValue("Test4", "Dalarö");
            service.SaveState(state);

            var loaded = service.LoadState(state.Name);
            loaded.Fields.Should().BeEquivalentTo(state.Fields);
            service.DeleteState(state.Name);
        }
    }
}