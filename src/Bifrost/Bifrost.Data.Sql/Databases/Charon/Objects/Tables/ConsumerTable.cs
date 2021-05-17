using Bifrost.Common.ApplicationServices.Sql.Common;

namespace Bifrost.Data.Sql.Databases.Bifrost.Objects.Tables
{
    internal class ConsumerTable : TableBase, IDatabaseObject
    {

        public static string TableName => "[dbo].[Consumers]";

        protected override string GetConstraintsSql()
        {
            return "";
        }

        protected override string GetCreateTableSql()
        {
            return string.Format("CREATE TABLE {0} (" +
                                 "Name nvarchar(256) primary key," +
                                 "ApiKey nvarchar(1024) NOT NULL UNIQUE," +
                                 "IsAdmin bit NOT NULL," +
                                 "CanEdit bit NOT NULL," +
                                 "JobsCsv nvarchar(2048)" +
                                 ")", TableName);

        }

        protected override string GetTableName()
        {
            return TableName;
        }

    }
}