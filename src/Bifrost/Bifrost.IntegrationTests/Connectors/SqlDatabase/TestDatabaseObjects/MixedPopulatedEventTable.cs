using System.Collections.Generic;
using Bifrost.Common.ApplicationServices.Sql.Common;

namespace Bifrost.IntegrationTests.Connectors.SqlDatabase.TestDatabaseObjects
{
    public class MixedPopulatedEventTable : TableBase, IDatabaseObject
    {
        public IList<string> Events { get; set; }
        public static string EventTypeColumnName = "EventType";

        public MixedPopulatedEventTable()
        {
            Events = new List<string>
            {
                "(N'first', 'add')",
                "(N'second', 'add')",
                "(N'third', 'delete')",
                "(N'fourth', 'delete')",
                "(N'fifth', 'add')"
            };
        }

        public static string TableName
        {
            get { return "[dbo].[MixedPopulatedEventTable]"; }
        }

        protected override string GetConstraintsSql()
        {
            return "";
        }

        protected override string GetCreateTableSql()
        {
            return string.Format("CREATE TABLE {0} (" +
                                 "Id int  IDENTITY(1,1) primary key," +
                                 "Name nvarchar(256)," +
                                 "EventType nvarchar(16) );" +
                                 "INSERT INTO {0} VALUES {1}",
                TableName,
                string.Join(",",Events));

        }

        protected override string GetTableName()
        {
            return TableName;
        }

        public void AddEvent(string mainTableId, string eventType)
        {
            Events.Add(string.Format("(N'{0}','{1}')", mainTableId, eventType));
        }
    }
}