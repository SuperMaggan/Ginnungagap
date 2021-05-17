using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Bifrost.Connector.SqlDatabase;
using Bifrost.Connector.SqlDatabase.DatabaseConnectorState;
using Bifrost.Connector.SqlDatabase.DatabaseIntegration;
using Bifrost.Connector.SqlDatabase.SqlServer;
using Bifrost.Core.Connectors.Configs;
using Bifrost.Core.Connectors.Configs.SqlDatabase;
using Bifrost.Core.Connectors.States;
using Bifrost.Core.Settings;
using Bifrost.Data.Sql;
using Bifrost.IntegrationTests.Connectors.SqlDatabase.TestDatabaseObjects;
using Bifrost.Common.ApplicationServices.Sql;
using Bifrost.Common.ApplicationServices.Sql.Common;
using Bifrost.Common.ApplicationServices.Sql.Databases;
using Bifrost.Common.Core.ApplicationServices;
using FluentAssertions;
using Xunit;

namespace Bifrost.IntegrationTests.Connectors.SqlDatabase
{
    public class SqlServerConnectorTests
    {
        private  IStateService _stateService;
        private SqlDatabaseConnector CreateConnector()
        {
            var commonSettings = new CommonSettings();

            var commonDbSettings = new CommonDatabaseSettings()
            {
                CommonConnection = new SqlSettings().BifrostConnection
            };
                
            _stateService = new SqlStateService(new StateDatabase(commonDbSettings));
            
            return new SqlDatabaseConnector(
                new SqlWorkTaskService(new WorkTaskDatabase(commonDbSettings)),
               _stateService,
                new IDatabaseIntegration[]
                {
                    new SqlServerIntegration(commonSettings, 
                    new IDataChangeDiscoverer[]
                    {
                        new ChangetableDataChangeDiscoverer(), 
                        new EventTableDataChangeDiscoverer()
                    })
                });
        }

        [Fact]
        public void CanCrawlFreshTableWithChangeTable()
        {
            var settings = new TestSettings();

            var db = new TestDatabase(settings, new EmptyTable());
            db.Setup();

            var connector = CreateConnector();
            connector.ResetConnector("CrawlFreshTableWithChangeTable");
            connector.ExecuteFetch(GetTestConfiguration(
                "CrawlFreshTableWithChangeTable",
                settings.TestSqlDatabaseConnection.ToString(),
                EmptyTable.TableName,
                new List<TableDetail>()
                {
                    new TableDetail()
                    {
                        TableName = EmptyTable.TableName,
                        PrimaryKeyIsInteger = false,
                        PrimaryKeyName = "Name"
                    }
                },
                new List<EventTable>()
                ));

            db.Destroy();
        }

        [Fact]
        public void CanCrawlFreshTableWithEventTable()
        {

            var settings = new TestSettings();

            var db = new TestDatabase(settings, new EmptyTable(), new EmptyEventTable());
            db.Setup();

            var connector = CreateConnector();
            connector.ResetConnector("CrawlFreshTableWithEventTable");
            connector.ExecuteFetch(GetTestConfiguration(
                "CrawlFreshTableWithEventTable",
                settings.TestSqlDatabaseConnection.ToString(),
                EmptyTable.TableName,
                new List<TableDetail>(),
                new List<EventTable>()
                {
                    new EventTable()
                    {
                        EventSequenceColumnName = "Id",
                        MainTableIdColumnName = "Name",
                        EventTypeColumnName = EmptyEventTable.EventTypeColumnName,
                        DeleteEventTypeValue = "delete",
                        TableName = EmptyEventTable.TableName
                    }
                }));

            db.Destroy();
        }

        [Fact]
        public void CanCrawlPopulatedTableWithEventTable()
        {

            var settings = new TestSettings();

            var db = new TestDatabase(settings, new PopulatedTable(), new PopulatedEventTable());
            db.Setup();
            try
            {
                var connector = CreateConnector();
                connector.ResetConnector("CrawlPopulatedTableWithEventTable");
                var configuration = GetTestConfiguration(
                    "CrawlPopulatedTableWithEventTable",
                    settings.TestSqlDatabaseConnection.ToString(),
                    PopulatedTable.TableName,
                    new List<TableDetail>(),
                    new List<EventTable>()
                {
                    new EventTable()
                    {
                        EventSequenceColumnName = "Id",
                        MainTableIdColumnName = "Name",
                        EventTypeColumnName = PopulatedEventTable.EventTypeColumnName,
                        DeleteEventTypeValue = "delete",
                        TableName = PopulatedEventTable.TableName
                    }
                });
                var sourceChanges = connector.ExecuteFetch(configuration);
                sourceChanges.Adds.Count.Should().Be(2);

                sourceChanges = connector.ExecuteFetch(configuration);
                sourceChanges.Adds.Count.Should().Be(0);
                sourceChanges.Deletes.Count.Should().Be(0);
            }
            finally
            {
                db.Destroy();
            }
        }

        [Fact]
        public void CanDetectChangesUsingHashedValues()
        {

            var settings = new TestSettings();

            var db = new TestDatabase(settings, new PopulatedTable());
            db.Setup();
            try
            {
                var connector = CreateConnector();
                connector.ResetConnector("CanDetectChangesUsingHashedValues");
                var configuration = GetTestConfiguration(
                    "CanDetectChangesUsingHashedValues",
                    settings.TestSqlDatabaseConnection.ToString(),
                    PopulatedTable.TableName,
                    new List<TableDetail>(),
                    new List<EventTable>()
                );

                var sourceChanges = connector.ExecuteFetch(configuration);
                sourceChanges.Adds.Count.Should().Be(2);

                sourceChanges = connector.ExecuteFetch(configuration);
                sourceChanges.Adds.Count.Should().Be(0);
                sourceChanges.Deletes.Count.Should().Be(0);
            }
            finally
            {
                db.Destroy();
            }
        }

        [Fact]
        public void WillIncludeBothAddsAndDeletesUsingEventTable()
        {

            var settings = new TestSettings();

            var eventTable = new MixedPopulatedEventTable();
            var db = new TestDatabase(settings, new MixedPopulatedTable(), eventTable);
            db.Setup();
            try
            {
                var connector = CreateConnector();
                connector.ResetConnector("WillIncludeBothAddsAndDeletesUsingEventTable");
                var configuration = GetTestConfiguration(
                    "WillIncludeBothAddsAndDeletesUsingEventTable",
                    settings.TestSqlDatabaseConnection.ToString(),
                    MixedPopulatedTable.TableName,
                    new List<TableDetail>(),
                    new List<EventTable>()
                {
                    new EventTable()
                    {
                        EventSequenceColumnName = "Id",
                        MainTableIdColumnName = "Name",
                        EventTypeColumnName = MixedPopulatedEventTable.EventTypeColumnName,
                        DeleteEventTypeValue = "delete",
                        TableName = MixedPopulatedEventTable.TableName
                    }
                });
                var sourceChanges = connector.ExecuteFetch(configuration);
                sourceChanges.Adds.Count.Should().Be(6);
                sourceChanges.Deletes.Count.Should().Be(0);

                sourceChanges = connector.ExecuteFetch(configuration);
                sourceChanges.Adds.Count.Should().Be(0);

                eventTable.AddEvent("first","add");
                eventTable.AddEvent("second", "delete");
                eventTable.AddEvent("third", "add");
                eventTable.AddEvent("fourth", "delete");
                db.UpdateDbObject(eventTable);
                sourceChanges = connector.ExecuteFetch(configuration);
                sourceChanges.Adds.Count.Should().Be(2,"Should've found two adds since we added two to the event table");
                sourceChanges.Deletes.Count.Should().Be(2, "Should've found two deletes since two was added to the event table");

            }
            finally
            {
                db.Destroy();
            }
        }


        [Fact]
        public void WillPauseAndSetErrorStateWhenInitializationError()
        {

            var settings = new TestSettings();
            var db = new TestDatabase(settings, new PopulatedTable(), new PopulatedEventTable());
            db.Setup();
            try
            {
                var connector = CreateConnector();
                
                connector.ResetConnector("WillPauseAndSetErrorStateWhenInitializationError");
                var configuration = GetTestConfiguration(
                    "WillPauseAndSetErrorStateWhenInitializationError",
                    settings.TestSqlDatabaseConnection.ToString(),
                    PopulatedTable.TableName,
                    new List<TableDetail>(),
                    new List<EventTable>()
                {
                    new EventTable()
                    {
                        EventSequenceColumnName = "Id",
                        MainTableIdColumnName = "Name",
                        EventTypeColumnName = PopulatedEventTable.EventTypeColumnName,
                        DeleteEventTypeValue = "delete",
                        TableName = PopulatedEventTable.TableName
                    }
                });
                configuration.ConnectionString = "ThisShouldNotWork";
                Action execute = () => connector.ExecuteFetch(configuration);
                execute.Should().Throw<ArgumentException>(
                    "The connection string is erroneous and should throw an exception");
                
                
                var state = new ConnectorStateBase(_stateService.LoadState(configuration.JobName));
                state.State.Should().Be(JobState.Paused);
            }
            finally
            {
                db.Destroy();
            }
        }





        [Fact]
        public void WillPauseAndSetErrorStateWhenMainTableFails()
        {

            var settings = new TestSettings();
            var db = new TestDatabase(settings, new PopulatedTable(), new PopulatedEventTable());
            db.Setup();
            try
            {
                var connector = CreateConnector();

                connector.ResetConnector("WillPauseAndSetErrorStateWhenIncrementalFails");
                var configuration = GetTestConfiguration(
                    "WillPauseAndSetErrorStateWhenIncrementalFails",
                    settings.TestSqlDatabaseConnection.ToString(),
                    PopulatedTable.TableName,
                    new List<TableDetail>(),
                    new List<EventTable>()
                {
                    new EventTable()
                    {
                        EventSequenceColumnName = "Id",
                        MainTableIdColumnName = "Name",
                        EventTypeColumnName = PopulatedEventTable.EventTypeColumnName,
                        DeleteEventTypeValue = "delete",
                        TableName = PopulatedEventTable.TableName
                    }
                });
                configuration.BatchSize = 1;
                Action execute = () => connector.ExecuteFetch(configuration);
                execute.Invoke(); //init should work fine 
                var state = new ConnectorStateBase(_stateService.LoadState(configuration.JobName));
                state.State.Should().Be(JobState.IncrementalCrawling);

                //We change the name of the table to something that does not exist
                configuration.MainTable.TableName = "I_dont exist";
                 execute.Should().Throw<SqlException>("The main table name does not exist");
                state = new ConnectorStateBase(_stateService.LoadState(configuration.JobName));
                state.State.Should().Be(JobState.Error, "Should be in error state since the Main table doesen't exist");

                //Invoking it a third time will make it revert to the last working state (IncrementalCrawling) just to 
                //yet again fail to read the main table and go back to Error
                execute.Should().Throw<SqlException>("The main table name still does not exist");
                state = new ConnectorStateBase(_stateService.LoadState(configuration.JobName));
                state.State.Should().Be(JobState.Error);


            }
            finally
            {
                db.Destroy();
            }
        }

        private SqlDatabaseConnectorJobConfiguration GetTestConfiguration(string name, string connectionString, string maintableName, List<TableDetail> changeTables,
            List<EventTable> eventTables)
        {
            return new SqlDatabaseConnectorJobConfiguration()
            {
                BatchSize = 100,
                ConnectionString = connectionString,
                MainTable = new TableDetail()
                {
                    PrimaryKeyName = "Name",
                    PrimaryKeyIsInteger = false,
                    TableName = maintableName
                },
                ChangeTables = changeTables,
                EventTables = eventTables,
                ForeignSources = new List<RelatingSource>(),
                IntegrationType = "SqlServerIntegration",
                JobName = name
            };
        }
    }
}
