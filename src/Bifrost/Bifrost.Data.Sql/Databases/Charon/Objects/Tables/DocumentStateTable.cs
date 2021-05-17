using Bifrost.Common.ApplicationServices.Sql.Common;

namespace Bifrost.Data.Sql.Databases.Bifrost.Objects.Tables
{
    internal class DocumentStateTable : TableBase, IDatabaseObject
    {

        public static string TableName => "[dbo].[DocumentStates]";

        protected override string GetConstraintsSql()
        {
            return BuildIndexSql("Category","VerifyDate");
        }

        protected override string GetCreateTableSql()
        {
            return string.Format("CREATE TABLE {0} (" +
                                 "DocumentId nvarchar(512) NOT NULL, " +
                                 "Category nvarchar(128) NOT NULL, " +
                                 "HashValue varchar(32) NOT NULL, " +
                                 "OptionalData nvarchar(512) NOT NULL, " +
                                 "LastUpdated datetime NOT NULL," +
                                 "LastVerified datetime not null, " +
                                 "VerifyDate datetime NOT NULL, " +
                                 "primary key(DocumentId, Category) )", TableName);

        }


        protected override string GetTableName()
        {
            return TableName;
        }


    }
}
