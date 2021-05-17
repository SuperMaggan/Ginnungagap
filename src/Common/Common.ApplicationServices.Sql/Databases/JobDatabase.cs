using System.Collections.Generic;
using Bifrost.Common.ApplicationServices.Sql.Common;
using Bifrost.Common.ApplicationServices.Sql.Tables;

namespace Bifrost.Common.ApplicationServices.Sql.Databases
{
    public class JobDatabase : DatabaseBase
    {
        public JobDatabase(CommonDatabaseSettings settings)
            : base(settings.CommonConnection, settings.AutomaticallyCreateDatabase)
        {
            DbObjects = new List<IDatabaseObject>
            {
                new JobTable()
            };



            if (settings.DestroyCommonDatabase)
                Destroy();


        }
    }
}