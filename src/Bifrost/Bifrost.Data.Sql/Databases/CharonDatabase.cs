using System.Collections.Generic;
using Bifrost.Data.Sql.Databases.Bifrost.Objects.StoredProcedures;
using Bifrost.Data.Sql.Databases.Bifrost.Objects.Tables;
using Bifrost.Common.ApplicationServices.Sql.Common;

namespace Bifrost.Data.Sql.Databases
{
    /// <summary>
    /// The only point of this class is to make sure that the 
    /// database is correctly set up, including all tables and indices
    /// </summary>
    public class AsgardDatabase : DatabaseBase
    {   
        public AsgardDatabase(SqlSettings settings)
            : base(settings.BifrostConnection, settings.AutomaticallyCreateDatabase)
        {
            DbObjects = new List<IDatabaseObject>()
                          {
                              new DocumentStateTable(),
                              new QueueItemTable(),
                              new ConsumerTable(),
                              new PushVerifyDocumentToQueueProcedure()
                          };
            
            if(settings.DestroyBifrostDatabase)
                Destroy();
       
        }

     
    }
}
