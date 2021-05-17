using Bifrost.Common.ApplicationServices.Sql.Common;

namespace Bifrost.IntegrationTests.Connectors.SqlDatabase.TestDatabaseObjects
{
    public class EmptyTable : TableBase, IDatabaseObject
    {

        public static string TableName
        {
            get { return "[dbo].[EmptyTable]"; }
        }

        protected override string GetConstraintsSql()
        {
            return string.Format("ALTER TABLE {0} ENABLE CHANGE_TRACKING", TableName);
        }

        protected override string GetCreateTableSql()
        {
            return string.Format("CREATE TABLE {0} (" +
                                 "Name nvarchar(256) primary key)", 
                TableName);

        }

        protected override string GetTableName()
        {
            return TableName;
        }

    }
}