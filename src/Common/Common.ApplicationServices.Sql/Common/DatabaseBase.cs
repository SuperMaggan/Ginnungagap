using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using Dapper;
using Bifrost.Common.Core.Settings;
using Serilog;

namespace Bifrost.Common.ApplicationServices.Sql.Common
{
    public abstract class DatabaseBase
    {
        private readonly bool _createDatabaseIfNotExist;
        private readonly string _connectionString;
        private bool _isSetup;
        protected IList<IDatabaseObject> DbObjects;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStringSettings"></param>
        /// <param name="createDatabaseIfNotExist">If true, the configured user need suffient rights</param>
        protected DatabaseBase(ConnectionStringSettings connectionStringSettings, bool createDatabaseIfNotExist)
        {
            _createDatabaseIfNotExist = createDatabaseIfNotExist;
            _connectionString = connectionStringSettings.ConnectionString;
            DbObjects = new List<IDatabaseObject>();
        }

        /// <summary>
        ///     Verifies the integrity of the database
        ///     Creates tables that does not exist
        /// </summary>
        /// <param name="conn"></param>
        public virtual void Setup(DbConnection conn)
        {
            var createdObjects = new List<IDatabaseObject>();
            
            foreach (var dbOject in DbObjects.Where(dbOject => !dbOject.Exists(conn)))
            {
                try
                {
                    dbOject.Create(conn);
                    createdObjects.Add(dbOject);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error when creating {name}", dbOject.GetType().Name);
                    throw;
                }
            }
            foreach (var dbOject in createdObjects)
            {
                try { 
                dbOject.SetConstraints(conn);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error when setting constraints on {name}", dbOject.GetType().Name);
                    throw;
                }
            }
            
            
            _isSetup = true;
        }

        /// <summary>
        ///     Verifies the integrity of the database
        ///     Creates tables that does not exist
        /// </summary>
        public void Setup()
        {
            CanConnect();
            Setup(GetOpenConnection());
        }

        /// <summary>
        ///     Will run only if Setup hasnt been executed yet
        ///     Verifies the integrity of the database
        ///     Creates tables that does not exist
        /// </summary>
        public void SetupOnce()
        {
            if (_isSetup)
                return;
            lock (_connectionString)
            {
                if (_isSetup)
                    return;
                Setup();
            }
        }

        public void Destroy()
        {
            Log.Information("Deleting database objects:");
            using (var connection = GetOpenConnection())
            {
                foreach (var databaseObject in DbObjects)
                {
                    databaseObject.Drop(connection);
                }
            }
        }

        public DbConnection GetOpenConnection()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            return conn;
        }

        protected int ExecuteSql(DbConnection connection, string sql)
        {
            return connection.Execute(sql);
        }

        private void CreateDatabaseIfNotExist()
        {
            var databaseName = GetDatabaseName(_connectionString);
            var masterConnStr = _connectionString.Replace(databaseName, "master");
            using (var conn = new SqlConnection(masterConnStr))
            {
                conn.Open();
                using (var reader = conn.ExecuteReader($"SELECT DB_ID (N'{databaseName}')"))
                {
                    reader.Read();
                    if (!reader.IsDBNull(0))
                        return;
                }
                conn.Execute($"CREATE DATABASE {databaseName}");
            }
        }

        private string GetDatabaseName(string connString)
        {
            var tokens = connString.Split(';');
            foreach (var token in tokens)
            {
                var keyValueToken = token.Split('=');
                if (keyValueToken.Length != 2)
                    throw new FormatException("Bad format on the connection string when trying to parse the database name: " + connString);
                if (keyValueToken[0].Equals("Database", StringComparison.CurrentCultureIgnoreCase))
                    return keyValueToken[1];
            }
            throw new FormatException("Failed to find the database= keyvalue when trying to parse the database name: " + connString);
        }

        public bool CanConnect()
        {
            int tries = 1;
            while (true) { 
                using (var conn  = new SqlConnection(_connectionString))
                {
                    try
                    {
                        conn.Open();
                        Log.Information("Successfully connected to database!");
                        return true;
                    }
                    catch (SqlException e)
                    {

                        Log.Warning("{tries}: Can not connect to database at with {_connectionString}: {message}", tries, _connectionString, e.Message);

                        if (_createDatabaseIfNotExist)
                        {
                            Log.Information("Trying to create database..");
                            CreateDatabaseIfNotExist();
                        }
                        tries++;
                        if(tries > 15) { 
                            Log.Error("Can not connect to database: {message}. Aborting.", e.Message);
                            throw;
                        }
                        Thread.Sleep(5000);
                    }
                }
            }
        }
    }
}