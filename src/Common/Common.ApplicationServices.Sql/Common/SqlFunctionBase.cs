using System.Data;
using System.Data.Common;
using System.Linq;
using Dapper;

namespace Bifrost.Common.ApplicationServices.Sql.Common
{
    public abstract class SqlFunctionBase
    {
        protected abstract string GetCreateSql();
        protected abstract string GetName();

        public bool Exists(DbConnection connection)
        {
            OpenConnectionIfClosed(connection);
            var count = connection.Query<int>(
                string.Format(
                    "SELECT COUNT(*) FROM sysobjects WHERE name = '{0}' AND type in (N'FN', N'IF', N'TF', N'FS', N'FT')",
                    GetName())).FirstOrDefault();
            return count > 0;
        }

        public void Create(DbConnection connection)
        {
            OpenConnectionIfClosed(connection);
            if (connection.State != ConnectionState.Open)
                connection.Open();
            connection.Execute(GetCreateSql());
        }

        public void Drop(DbConnection connection)
        {
            OpenConnectionIfClosed(connection);
            connection.Execute(string.Format("DROP FUNCTION {0}", GetName()));
        }

        private void OpenConnectionIfClosed(DbConnection conn)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
        }
    }
}