using System.Collections.Generic;
using Bifrost.Core.Connectors.Configs.SqlDatabase;
using Bifrost.Common.Core.Domain.Jobs;

namespace Bifrost.Core.Connectors.Configs
{
    public class SqlDatabaseConnectorJobConfiguration : JobConfigurationBase
    {
        public override string Description
            => "Extracts textual data found in relational databases, eg. MS SQL Server and Oracle";
        public override string DisplayName => "Sql database connector";


        public SqlDatabaseConnectorJobConfiguration()
        {
            ChangeTables =new List<TableDetail>();
            EventTables = new List<EventTable>();
            ForeignSources = new List<RelatingSource>();


        }
        

        public string ConnectionString { get; set; }

        /// <summary>
        /// Type of Integration to use, eg. OracleIntegration or SqlServerIntegration
        /// </summary>
        public string IntegrationType { get; set; }

        /// <summary>
        /// How many rows that should be selected at the time. 
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// If set, the connector will reset its state every x hour
        /// Used if no change/event tables are utilized and you want to do a 
        /// full recrawl ever X hour
        /// 0 is the same as 'never reset'
        /// </summary>
        public int ResetEveryXHour { get; set; }

        /// <summary>
        /// The main table or view that all data is SELECTed from
        /// </summary>
        public TableDetail MainTable { get; set; }

        /// <summary>
        /// Tables that has changetable enabled. 
        /// If one or more changetables are defined here then they will
        /// be polled for any changes made. If a change is found then the appropriate
        /// row in the MainTable will be read
        /// This technique is only used by MS Sql Server
        /// </summary>
        public List<TableDetail> ChangeTables { get; set; }

        /// <summary>
        /// If you dont use changetables (you might *gasp* not even use MS SQL Server)
        /// You can configure how delta crawls should be performed using EventTables 
        /// </summary>
        public List<EventTable> EventTables { get; set; }
        
        /// <summary>
        /// These are used if you need to join data from a another table. The table
        /// can even be in a completely different database
        /// </summary>
        public List<RelatingSource> ForeignSources { get; set; }

        

        public override IJobConfiguration CreateExample()
        {
            return new SqlDatabaseConnectorJobConfiguration()
            {
                BatchSize = 100,
                ConnectionString = "Server=SECC5399;Database=Asgard;Trusted_Connection=True;Connection Timeout=200",
                IntegrationType = "OracleIntegration",
                ResetEveryXHour = -1,
                MainTable = new TableDetail()
                {
                    TableName = "Documents",
                    PrimaryKeyIsInteger = false,
                    PrimaryKeyName = "Id"
                },
                ChangeTables = new List<TableDetail>()
                {
                    new TableDetail()
                    {
                        TableName = "Document",
                        PrimaryKeyIsInteger = false,
                        PrimaryKeyName = "Id"
                    }
                },
                EventTables = new List<EventTable>()
                {
                    new EventTable()
                    {
                        TableName = "Document_Changes",
                        DeleteEventTypeValue = "DELETE",
                        EventSequenceColumnName = "SEQ",
                        EventTypeColumnName = "ACTION",
                        MainTableIdColumnName = "ID"
                    }
                }
            };
        }
    }
}