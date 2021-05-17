using System.Data.Common;
using System.Linq;
using Bifrost.Common.ApplicationServices.Sql.Common;

namespace Bifrost.IntegrationTests.Connectors.SqlDatabase.TestDatabaseObjects
{
    public class TestDatabase : DatabaseBase
    {
        public TestDatabase(TestSettings settings)
            : this(settings, new EmptyTable())
        { }

        public TestDatabase(TestSettings settíngs, params IDatabaseObject[] dbObjects)
            : base(settíngs.TestSqlDatabaseConnection, true)
        {
            DbObjects = dbObjects.ToList();
        }

        /// <summary>
        /// Verifies the integrity of the database
        /// Creates tables that does not exist
        /// </summary>
        /// <param name="conn"></param>
        public override void Setup(DbConnection conn)
        {

            try
            {
                ExecuteSql(conn,
                    string.Format("ALTER DATABASE {0} SET CHANGE_TRACKING = ON", conn.Database));
            }
            catch //Supress any errors
            {
                
            }

            base.Setup(conn);

        }

        public void UpdateDbObject(IDatabaseObject dbObject)
        {
            var conn = GetOpenConnection();
            if(dbObject.Exists(conn))
                dbObject.Drop(conn);
            dbObject.Create(conn);
            dbObject.SetConstraints(conn);
        }
            
    }


}
