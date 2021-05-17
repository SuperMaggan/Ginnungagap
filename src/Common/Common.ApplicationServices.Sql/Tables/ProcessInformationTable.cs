

using Bifrost.Common.ApplicationServices.Sql.Common;

namespace Bifrost.Common.ApplicationServices.Sql.Tables
{
    internal class ProcessInformationTable : TableBase, IDatabaseObject
    {
        public static string TableName => "[dbo].[ProcessInformation]";

        protected override string GetConstraintsSql()
        {
            return "";
        }

        protected override string GetCreateTableSql()
        {
            return string.Format("CREATE TABLE {0} (" +
                                 "Id nvarchar(256) primary key," +
                                 "ProcessType nvarchar(128) NOT NULL," +
                                 "ServerName nvarchar(128) NOT NULL," +
                                 "LastUpdated datetime NOT NULL," +
                                 "UpdateFrequencySec int NOT NULL,  " +
                                 "InformationCsv nvarchar(MAX)" +
                                 ")", TableName);
        }

        protected override string GetTableName()
        {
            return TableName;
        }
    }
}