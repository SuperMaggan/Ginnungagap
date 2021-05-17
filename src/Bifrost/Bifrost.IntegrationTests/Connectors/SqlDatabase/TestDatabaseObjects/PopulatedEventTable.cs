
using Bifrost.Common.ApplicationServices.Sql.Common;

namespace Bifrost.IntegrationTests.Connectors.SqlDatabase.TestDatabaseObjects
{
    public class PopulatedEventTable : TableBase, IDatabaseObject
    {

        public static string EventTypeColumnName = "EventType";
        
        public static string TableName
        {
            get { return "[dbo].[PopulatedEventTable]"; }
        }

        protected override string GetConstraintsSql()
        {
            return "";
        }

        protected override string GetCreateTableSql()
        {
            return string.Format("CREATE TABLE {0} (" +
                                 "Id int primary key, Name nvarchar(256), EventType nvarchar(16) );" +
                                 "INSERT INTO {0} VALUES (1, N'first', 'add'), (2, N'second', 'add')",
                TableName);

        }

        protected override string GetTableName()
        {
            return TableName;
        }

    }
}