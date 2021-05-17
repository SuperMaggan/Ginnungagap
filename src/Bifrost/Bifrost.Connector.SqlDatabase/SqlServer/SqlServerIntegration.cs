using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Bifrost.Connector.SqlDatabase.DatabaseConnectorState;
using Bifrost.Connector.SqlDatabase.DatabaseConnectorState.ChangeTracking;
using Bifrost.Connector.SqlDatabase.DatabaseIntegration;
using Bifrost.Core.Connectors.Configs;
using Bifrost.Core.Connectors.Configs.SqlDatabase;
using Bifrost.Core.Connectors.States.SqlDatabase;
using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Document;
using Bifrost.Core.Settings;
using Bifrost.Common.Core.Domain;
using Serilog;

namespace Bifrost.Connector.SqlDatabase.SqlServer
{

    public class SqlServerIntegration : IDatabaseIntegration
    {
        private readonly CommonSettings _commonSettings;
        private readonly IDataChangeDiscoverer[] _changeDiscoverers;

        public SqlServerIntegration(CommonSettings commonSettings, IDataChangeDiscoverer[] changeDiscoverers)
        {
            _commonSettings = commonSettings;
            _changeDiscoverers = changeDiscoverers;
        }


        public IEnumerable<string> DiscoverInitialIds(SqlDatabaseConnectorJobConfiguration config)
        {
            if(config.MainTable == null)
                throw new MissingFieldException("A main table needs to be configured in order to know where to get the data from");
            using (var conn = new SqlConnection(config.ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    string.Format("SELECT {0} FROM {1}",
                                    config.MainTable.PrimaryKeyName,
                                  config.MainTable.TableName);
                Log.Logger.Debug($"{config.JobName}: Discovering Ids with: {cmd.CommandText}");
                conn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    int idOrdinal = -1;
                    while (dr.Read())
                    {
                        if (idOrdinal == -1)
                            idOrdinal = dr.GetOrdinal(config.MainTable.PrimaryKeyName);

                        yield return dr.GetValue(idOrdinal).ToString();
                    }
                }
            }
        }


        public IEnumerable<string> DiscoverMainTableIds(SqlDatabaseConnectorJobConfiguration config, string constraintColumn, IList<string> foreignIds)
        {
            using (var conn = new SqlConnection(config.ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    string.Format("SELECT DISTINCT {0} FROM {1} WHERE {2}",
                                  config.MainTable.PrimaryKeyName,
                                  config.MainTable.TableName,
                                  CreateWhereConstraint(foreignIds, constraintColumn, config.MainTable.PrimaryKeyIsInteger));
                Log.Logger.Debug($"{config.JobName}: Fetching initial document batch with: {cmd.CommandText}");
                conn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int idOrdinal = -1;
                        while (dr.Read())
                        {
                            if (idOrdinal == -1)
                                idOrdinal = dr.GetOrdinal(config.MainTable.PrimaryKeyName);
                            yield return dr.GetValue(idOrdinal).ToString();
                        }
                    }
                }
            }
        }

        public IEnumerable<AddDocument> FetchDocuments(SqlDatabaseConnectorJobConfiguration config, string constraintColumn, bool constraintIsIntType, IEnumerable<string> idsToSelect)
        {
            using (var conn = new SqlConnection(config.ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    string.Format("SELECT * FROM {0} WHERE {1}",
                                  config.MainTable.TableName,
                                  CreateWhereConstraint(idsToSelect, constraintColumn, constraintIsIntType));
                Log.Logger.Debug($"{config.JobName}: Fetching initial document batch with: {cmd.CommandText}");
                conn.Open();
                
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        
                        var document = DataRowToDocument(dr, constraintColumn,config.JobName);
                        if (document == null)
                            yield break;
                        yield return document;
                    }
                }
            }

        }

        public long GetLastChangeVersion(SqlDatabaseConnectorJobConfiguration config, TableDetail changeTableSettings)
        {
            using (var conn = new SqlConnection(config.ConnectionString))
            using (var cmd = conn.CreateCommand())
            {

                cmd.CommandText =
                    string.Format(
                        "SELECT TOP ( 1 ) SYS_CHANGE_VERSION FROM CHANGETABLE (CHANGES {0}, 0) as CT ORDER BY SYS_CHANGE_VERSION desc",
                        changeTableSettings.TableName);
                Log.Logger.Debug($"{config.JobName}: Getting the latest version number with: {cmd.CommandText}");

                try
                {
                    conn.Open();
                    var scalar = cmd.ExecuteScalar();
                    if (scalar != null)
                        return (long)scalar;

                }
                catch (SqlException e)
                {
                    Log.Error(e, $"{config.JobName}: SQL error when executing {cmd.CommandText}");
                    throw;
                }
            }
            return 0;
       }

        public long GetLastChangeVersion(SqlDatabaseConnectorJobConfiguration config, EventTable eventTableSettings)
        {
            using (var conn = new SqlConnection(config.ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                   string.Format(
                       "SELECT MAX({1}) FROM {0}",
                       eventTableSettings.TableName,
                       eventTableSettings.EventSequenceColumnName);
                Log.Logger.Debug($"{config.JobName}: Getting the latest version number with: {cmd.CommandText}");

                try
                {
                    conn.Open();
                    var scalar = cmd.ExecuteScalar();
                    if (scalar.GetType().GetTypeInfo().IsValueType)
                    {
                        return (int) scalar;
                    }
                    return 0;
                }
                catch (SqlException e)
                {
                    Log.Error(e, $"{config.JobName}: SQL error when executing {cmd.CommandText}");
                    throw;
                }
            }
        }



        public IEnumerable<string> DiscoverIncrementalIds(SqlDatabaseConnectorJobConfiguration config, ref DatabaseJobState stateBase, EventType changeType)
        {

            var discoverer = _changeDiscoverers.FirstOrDefault(x => x.CanHandle(config));
            if (discoverer == null)
            {
                Log.Debug("{jobName}: No incremental strategy for {changeType} (no discoverer registered that handles the configuration), no delta crawls will be possible.", config.JobName, changeType);
                return new List<string>();
            }
            var dataChanges = discoverer.DiscoverChanges(config, stateBase).ToList();

            foreach (var dataChange in dataChanges)
            {
                if(!dataChange.AddedIds.Any() && !dataChange.DeletedIds.Any())
                    continue;
                stateBase.SetChangetableVersionThreshold(dataChange.Table.TableName, dataChange.HighestVersionAdded, true);
            }

            return dataChanges.SelectMany(
                dataChange => GetDataChangesInMainTable(config, dataChange, changeType));
            
        }

        /// <summary>
        /// Using a DataChange object containing changes in a changeTable, this function
        /// discovers which rows (using the MainTables idColumn) that has changed in the
        /// main table
        /// </summary>
        /// <param name="config"></param>
        /// <param name="change"></param>
        /// <param name="changeType"></param>
        /// <returns></returns>
        private IEnumerable<string> GetDataChangesInMainTable(SqlDatabaseConnectorJobConfiguration config, DataChange change, EventType changeType)
        {
            var ids = changeType == EventType.Add ? change.AddedIds : change.DeletedIds;
            if(!ids.Any())
                yield break;
            if (config.MainTable.PrimaryKeyName == change.Table.PrimaryKeyName) //if the column names are the same, its a fair assumption to say they are a direct mapping
                foreach(var id in ids)
                    yield return id;
            var constraintBatches = CreateTSqlWhereConstraintBatches(
                change.Table.PrimaryKeyName, ids, change.Table.PrimaryKeyIsInteger, config.BatchSize);
            foreach (var constraintBatch in constraintBatches.Where(x => !string.IsNullOrEmpty(x)))
            {
                using (var conn = new SqlConnection(config.ConnectionString))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        string.Format("SELECT DISTINCT {0} FROM {1} WHERE {2}",
                                        config.MainTable.PrimaryKeyName,
                                      config.MainTable.TableName,
                                      constraintBatch);
                    cmd.CommandTimeout = 300;
                    Log.Logger.Debug($"{config.JobName}: Fetching changed documents with: {cmd.CommandText}");

                    conn.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        int idOrdinal = -1;
                        while (dr.Read())
                        {
                            if (idOrdinal == -1)
                                idOrdinal = dr.GetOrdinal(config.MainTable.PrimaryKeyName);

                            yield return dr.GetValue(idOrdinal).ToString();
                        }
                    }

                }
            }
        }
        


      





        private AddDocument DataRowToDocument(SqlDataReader reader, string idColumnName, string configJobName)
        {
            int idOrdinal;
            try
            {
                idOrdinal = reader.GetOrdinal(idColumnName);
            }
            catch (IndexOutOfRangeException)
            {
                Log.Logger.Error($"Encountered a data row lacking the required id column {idColumnName} ");
                return null;
            }

            var id = reader.GetValue(idOrdinal).ToString();
            //var document = new Document(string.Format("{0}_{1}", jobSettings.DataViewSettings.ViewName, id), jobSettings.JobGroup);
            var document = new AddDocument(id, configJobName);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.IsDBNull(i))
                    continue;
                
                if (reader.GetFieldType(i).Name == "Byte[]"){
                    document.Fields.Add(new BinaryField(reader.GetName(i), reader.GetValue(i) as byte[]));
                }
                else if (reader.GetFieldType(i).Name == "DateTime")
                {
                    var strValue = string.IsNullOrEmpty(_commonSettings.DateTimeFormat)
                        ? reader.GetDateTime(i).ToString(CultureInfo.InvariantCulture)
                        : reader.GetDateTime(i).ToString(_commonSettings.DateTimeFormat);
                    document.Fields.Add(new Field(reader.GetName(i), strValue));
                }
                else { 
                    document.Fields.Add(new Field(reader.GetName(i), Convert.ToString(reader.GetValue(i), CultureInfo.InvariantCulture)));
                }
            }
            return document;
        }
     

       





        /// <summary>
        /// Will create a string to be used in WHERE constraints
        /// <example>CustomerId IN {1, 3 ,232321, 3}</example>
        /// <example>CustomerAdressId IN {'1', '3' ,'232321', '3'}</example>
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="idColumnName"></param>
        /// <param name="isIntType"></param>
        /// <returns></returns>
        private string CreateWhereConstraint(IEnumerable<string> ids, string idColumnName, bool isIntType)
        {
            if (ids == null)
                return "";
            var sb = new StringBuilder();
            sb.AppendFormat("{0} IN ", idColumnName);
            if (isIntType)
                sb.AppendFormat("( {0} )", string.Join(",", ids));
            else
                sb.AppendFormat("( '{0}' )", string.Join("','", ids.Select(s => s.Replace("'", "''"))));
            return sb.ToString();
        }


        /// <summary>
        /// Creates a where constraint string using datachanges from a changeTable
        /// This could be optimized..
        /// </summary>
        /// <param name="eventType"> </param>
        /// <param> <name>dataChanges</name> </param>
        /// <returns>
        /// <example>Id IN (2,31,1,34,1,23,123,123,) OR Id2 IN (2223,32,332)</example>
        /// </returns>
        private IEnumerable<string> CreateTSqlWhereConstraintBatches(
            string idColumnName, IList<string> ids, bool isIntType, int batchLimit)
        {
            var sb = new StringBuilder();
            const string intValueFormat = "{0}";
            const string stringValueFormat = "'{0}'";

            int numBatches = (ids.Count / batchLimit) + ((ids.Count % batchLimit) > 0 ? 1 : 0);

            for (int factor = 0; factor < numBatches; factor++)
            {
                string format = isIntType ? intValueFormat : stringValueFormat;
                yield return
                    string.Format("{0} IN ( {1} )",
                    idColumnName,
                    string.Join(",",
                            ids.Skip(factor * batchLimit)
                            .Take(batchLimit)
                            .Select(x => string.Format(format, x.Replace("'","''")))
                        ));
            }
            yield return sb.ToString();
            
        }

    }
}
