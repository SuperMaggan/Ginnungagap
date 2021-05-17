using Bifrost.Common.ApplicationServices.Sql.Common;

namespace Bifrost.Common.ApplicationServices.Sql.Tables
{
    public class ScriptsTable : TableBase, IDatabaseObject
    {
        public static string TableName => "Scripts";

        protected override string GetConstraintsSql() => "";

        protected override string GetCreateTableSql()
        {
            return string.Format("CREATE TABLE [dbo].[{0}] (" +
                                 "Name nvarchar(128) PRIMARY KEY," +
                                 "DomainName nvarchar(128)," +
                                 "LastUpdated datetime NOT NULL, " +
                                 "Metadata nvarchar(2048), " +
                                 "ImportedScriptNamesCsv nvarchar(256), " +
                                 "Owner nvarchar(128), " +
                                 "IsPublic bit, " +
                                 "Code nvarchar(MAX)" +
                                 ")", TableName);
        }

        protected override string GetTableName()
        {
            return TableName;
        }
    }
}