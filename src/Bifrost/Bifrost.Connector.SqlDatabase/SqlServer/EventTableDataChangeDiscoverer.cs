using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Bifrost.Connector.SqlDatabase.DatabaseConnectorState;
using Bifrost.Connector.SqlDatabase.DatabaseConnectorState.ChangeTracking;
using Bifrost.Core.Connectors.Configs;
using Bifrost.Core.Connectors.Configs.SqlDatabase;
using Bifrost.Core.Connectors.States.SqlDatabase;
using Bifrost.Core.Domain;
using Serilog;

namespace Bifrost.Connector.SqlDatabase.SqlServer
{
    public class EventTableDataChangeDiscoverer : IDataChangeDiscoverer
    {
        public IEnumerable<DataChange> DiscoverChanges(SqlDatabaseConnectorJobConfiguration config, DatabaseJobState stateBase)
        {
            return DiscoverDataChangesUsingEventTables(config, stateBase);
        }

        public bool CanHandle(SqlDatabaseConnectorJobConfiguration config)
        {
            return config.EventTables.Any();
            
        }

        private IEnumerable<DataChange> DiscoverDataChangesUsingEventTables(SqlDatabaseConnectorJobConfiguration config, DatabaseJobState stateBase)
        {
            foreach (var eventTable in config.EventTables)
            {
                var dataChange = new DataChange(new TableDetail()
                {
                    TableName = eventTable.TableName,
                    PrimaryKeyName = eventTable.MainTableIdColumnName

                });

                using (var conn = new SqlConnection(config.ConnectionString))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        string
                            .Format(
                                "SELECT {0}, {1}, {2} FROM {3} WHERE {0} > {4} ORDER BY {0} asc",
                                eventTable.EventSequenceColumnName,
                                eventTable.MainTableIdColumnName,
                                eventTable.EventTypeColumnName,
                                eventTable.TableName,
                                stateBase.GetChangetableVersionThreshold(eventTable.TableName));
                    Log.Logger.Debug($"{config.JobName}: Getting data changes from changetables with: {cmd.CommandText}");

                    try
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                                dataChange.AddChange(DataRowToChangeTableEvent(reader, eventTable));
                        }
                    }
                    catch (SqlException e)
                    {

                        Log.Logger.Error($"{config.JobName}: SQL error when executing {cmd.CommandText}", e);
                        throw;
                    }
                }
                yield return dataChange;
            }
        }

        private ChangeTableEvent DataRowToChangeTableEvent(SqlDataReader reader, EventTable eventTable)
        {
            long version;

            if (!long.TryParse(reader.GetValue(reader.GetOrdinal(eventTable.EventSequenceColumnName)).ToString(), out version))
                throw new MissingFieldException(
                    string.Format("Failed to parse ordinal {1} as a long ({0})",
                        eventTable.TableName,
                        eventTable.EventSequenceColumnName));

            int primaryKeyIdOrdinal;
            try
            {
                primaryKeyIdOrdinal = reader.GetOrdinal(eventTable.MainTableIdColumnName);

            }
            catch (IndexOutOfRangeException)
            {
                throw new MissingFieldException(string.Format("Failed to find ordinal (or empty) {1} (for table {0})",
                    eventTable.TableName,
                    eventTable.MainTableIdColumnName));
            }
            var changedId = reader.GetValue(primaryKeyIdOrdinal).ToString();
            var eventStr = reader[eventTable.EventTypeColumnName] as string;
            if (string.IsNullOrEmpty(eventStr))
                throw new MissingFieldException(
                    string.Format("Failed to find ordinal (or empty) {1} ({0})", eventTable.TableName, eventTable.EventTypeColumnName));
            EventType eventType = eventStr.Equals(eventTable.DeleteEventTypeValue, StringComparison.OrdinalIgnoreCase) ? EventType.Delete : EventType.Add;

            return new ChangeTableEvent(version, changedId, eventType);
        }
    }
}