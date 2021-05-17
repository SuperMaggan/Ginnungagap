using Bifrost.Common.ApplicationServices.Sql.Common;

namespace Bifrost.Data.Sql.Databases.Bifrost.Objects.Tables
{
    internal class QueueItemTable : TableBase, IDatabaseObject
    {

        public static string TableName => "[dbo].[QueueItems]";

        protected override string GetConstraintsSql()
        {
            return BuildIndexSql("Category");
        }

        protected override string GetCreateTableSql()
        {
            return string.Format("CREATE TABLE {0} (" +
                                 "Id int IDENTITY(1,1) primary key," +
                                 "Category nvarchar(128) NOT NULL," +
                                 "CreateDate datetime NOT NULL," +
                                 "DocumentId nvarchar(512) NOT NULL, " +
                                 "OptionalData nvarchar(2048)" +
                                 ")"
                                 ,TableName);

        }

        protected override string GetTableName()
        {
            return TableName;
        }

    }
}
