using System.Data.Common;

namespace Bifrost.Common.ApplicationServices.Sql.Common
{
    public interface IDatabaseObject
    {
        bool Exists(DbConnection connection);
        void Create(DbConnection connection);
        void SetConstraints(DbConnection connection);
        void Drop(DbConnection connection);
    }
}