using Bifrost.Common.ApplicationServices.Sql.Common;

namespace Bifrost.Common.ApplicationServices.Sql.Tables
{
    public class JobTable : TableBase, IDatabaseObject
    {
        public static string TableName => "[dbo].[Jobs]";
        
        protected override string GetConstraintsSql()
        {
            return "";
        }

        protected override string GetCreateTableSql()
        {
            return string.Format(@"
                    CREATE TABLE {0} (
	                    [Name] [nvarchar](256) NOT NULL,
	                    [Description] [nvarchar](1000),
                        [JobType] [nvarchar](300) NOT NULL,
                        [TriggerCronSyntax] [nvarchar](200) NOT NULL,
                        [Configuration] [nvarchar](4000),
                        [Enabled] bit NOT NULL,
                        [ConcurrentLimit] int NOT NULL,
                        [LastUpdated] datetime)", TableName);
        }

        protected override string GetTableName()
        {
            return TableName;
        }
    }
}