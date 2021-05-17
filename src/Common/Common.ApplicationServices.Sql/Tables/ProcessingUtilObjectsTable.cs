using Bifrost.Common.ApplicationServices.Sql.Common;

namespace Bifrost.Common.ApplicationServices.Sql.Tables
{
    public class ProcessingUtilObjectsTable : TableBase, IDatabaseObject
    {
        public static string TableName => "[dbo].[ProcessingUtilObjects]";

        protected override string GetConstraintsSql()
        {
            return @"";
        }

        protected override string GetCreateTableSql()
        {
            return string.Format((string) ("CREATE TABLE {0} (" +
                                           "[Key] nvarchar(400) primary key, " +
                                           "ObjectType nvarchar(128) NOT NULL, " +
                                           "Value nvarchar(MAX))")
                , (object) TableName);
        }

        protected override string GetTableName()
        {
            return TableName;
        }
    }
}