using Bifrost.Common.ApplicationServices.Sql.Common;

namespace Bifrost.Common.ApplicationServices.Sql.Tables
{
    internal class WorkTaskTable : TableBase, IDatabaseObject
    {
        public static string TableName => "[dbo].[WorkTasks]";

        protected override string GetConstraintsSql()
        {
            return $@"
                    CREATE INDEX [IX_WorkTask_owner_checkedOut] 
                    ON {TableName} ([Owner], [CheckedOut])
                    INCLUDE ([Id],[InstructionsCsv])
                ";
        }

        protected override string GetCreateTableSql()
        {
            return string.Format("CREATE TABLE {0} (" +
                                 "Id int IDENTITY(1,1) primary key," +
                                 "Owner nvarchar(128) NOT NULL," +
                                 "CheckedOut bit NOT NULL," +
                                 "InstructionsCsv nvarchar(MAX)" +
                                 ")", TableName);
        }

        protected override string GetTableName()
        {
            return TableName;
        }
    }
}