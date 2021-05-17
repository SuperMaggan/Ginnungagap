using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;

namespace Bifrost.Common.ApplicationServices.Sql.Common
{
    public abstract class TableBase
    {
        protected abstract string GetConstraintsSql();
        protected abstract string GetCreateTableSql();
        protected abstract string GetTableName();

        /// <summary>
        ///     Gets the table name without dbo namespace and brackets
        ///     [dbo].[jobs] -> jobs
        /// </summary>
        /// <returns></returns>
        protected virtual string GetPlainTableName()
        {
            return GetTableName().Replace("[dbo].", "").Replace("[", "").Replace("]", "");
        }

        public bool Exists(DbConnection connection)
        {
            OpenConnectionIfClosed(connection);
            if (connection is SqlConnection)
            {
                var sql = string.Format("(SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{0}')",
                    GetPlainTableName());
                var count = connection.Query<int>(sql).FirstOrDefault();
                return count > 0;
            }
            throw new NotSupportedException("Connection of type " + connection.GetType().Name + " is not supported.");
        }

        public void Create(DbConnection connection)
        {
            OpenConnectionIfClosed(connection);
            if (connection.State != ConnectionState.Open)
                connection.Open();
            connection.Execute(GetCreateTableSql());
        }

        public void SetConstraints(DbConnection connection)
        {
            var sql = GetConstraintsSql();
            if (string.IsNullOrEmpty(sql))
                return;

            OpenConnectionIfClosed(connection);
            if (connection.State != ConnectionState.Open)
                connection.Open();
            connection.Execute(sql);
        }

        public void Drop(DbConnection connection)
        {
            OpenConnectionIfClosed(connection);
            connection.Execute(string.Format("DROP TABLE {0}", GetTableName()));
        }

        private void OpenConnectionIfClosed(DbConnection conn)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
        }


        protected string BuildIndexSql(params string[] columns)
        {
            var sql = new StringBuilder();
            foreach (var column in columns)
            {
                var cleanTableName = GetTableName().Replace("[", "").Replace("]", "").Replace(".", "_");
                sql.AppendLine($"CREATE INDEX IX_{cleanTableName}_{column} " +
                               $"ON {GetTableName()} ({column}) " +
                               $"");
            }
            return sql.ToString();
        }
    }
}