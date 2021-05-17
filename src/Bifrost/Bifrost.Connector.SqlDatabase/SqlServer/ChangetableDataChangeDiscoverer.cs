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
    public class ChangetableDataChangeDiscoverer : IDataChangeDiscoverer {
        public IEnumerable<DataChange> DiscoverChanges(SqlDatabaseConnectorJobConfiguration config, DatabaseJobState stateBase)
        {
            return DiscoverDataChangesUsingChangetables(config, stateBase);
        }

        public bool CanHandle(SqlDatabaseConnectorJobConfiguration config)
        {
            return config.ChangeTables.Any();
        }

        /// <summary>
        /// Discover all changes listed in the change tables
        /// </summary>
        /// <param name="config"></param>
        /// <param name="stateBase"></param>
        /// <returns></returns>
        private IEnumerable<DataChange> DiscoverDataChangesUsingChangetables(SqlDatabaseConnectorJobConfiguration config, DatabaseJobState stateBase)
        {
            foreach (var changeTable in config.ChangeTables)
            {
                var dataChange = new DataChange(changeTable);
                using (var conn = new SqlConnection(config.ConnectionString))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        string.Format(
                            "SELECT * FROM CHANGETABLE (CHANGES {0}, {1}) as CT ORDER BY SYS_CHANGE_VERSION asc",
                            changeTable.TableName,
                            stateBase.GetChangetableVersionThreshold(changeTable.TableName));
                    Log.Logger.Debug($"{config.JobName}: Getting data changes from changetables with: {cmd.CommandText}");

                    try
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                                dataChange.AddChange(DataRowToChangeTableEvent(reader, changeTable));
                        }
                    }
                    catch (SqlException e)
                    {

                        Log.Error(e,$"{config.JobName}: SQL error when executing {cmd.CommandText}");
                        throw;
                    }
                }
                yield return dataChange;
            }
        }
        private ChangeTableEvent DataRowToChangeTableEvent(SqlDataReader reader, TableDetail changeTable)
        {
            long version;

            if (!long.TryParse(reader.GetValue(reader.GetOrdinal("SYS_CHANGE_VERSION")).ToString(), out version))
                throw new MissingFieldException(string.Format("Failed to parse ordinal SYS_CHANGE_VERSION as a long ({0})",
                    changeTable.TableName));

            int primaryKeyIdOrdinal = -1;
            try
            {
                primaryKeyIdOrdinal = reader.GetOrdinal(changeTable.PrimaryKeyName);

            }
            catch (IndexOutOfRangeException)
            {
                throw new MissingFieldException(string.Format("Failed to find ordinal (or empty) {0} (for table {1})",
                    changeTable.PrimaryKeyName,
                    changeTable.TableName));
            }
            var changedId = reader.GetValue(primaryKeyIdOrdinal).ToString();
            var eventChar = reader["SYS_CHANGE_OPERATION"] as string;
            if (string.IsNullOrEmpty(eventChar))
                throw new MissingFieldException(
                    string.Format("Failed to find ordinal (or empty) SYS_CHANGE_OPERATION ({0})", changeTable.TableName));
            EventType eventType = eventChar.Equals("D") ? EventType.Delete : EventType.Add;

            return new ChangeTableEvent(version, changedId, eventType);
        }
    }
}
