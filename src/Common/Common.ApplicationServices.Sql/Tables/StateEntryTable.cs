using Bifrost.Common.ApplicationServices.Sql.Common;

namespace Bifrost.Common.ApplicationServices.Sql.Tables
{
    internal class StateEntryTable : TableBase, IDatabaseObject
    {
        public static string TableName => "[dbo].[StateEntries]";

        protected override string GetConstraintsSql()
        {
            return $@"
                    CREATE INDEX [IX_StateEntries_state] 
                    ON {TableName} ([StateName])
                    INCLUDE ([Id],[ParameterName],[ParameterValue])
                ";
        }

        protected override string GetCreateTableSql()
        {
            return string.Format("CREATE TABLE {0} (" +
                                 "Id int IDENTITY(1,1) primary key," +
                                 "StateName nvarchar(128) NOT NULL," +
                                 "ParameterName nvarchar(128) NOT NULL," +
                                 "ParameterValue nvarchar(MAX)" +
                                 ")", TableName);
        }

        protected override string GetTableName()
        {
            return TableName;
        }
    }
}