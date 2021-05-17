using System.Collections.Generic;
using Bifrost.Core.Connectors.Configs;
using Bifrost.Core.Connectors.Configs.SqlDatabase;
using Bifrost.Core.Connectors.States.SqlDatabase;
using Bifrost.Core.Domain;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Connector.SqlDatabase.DatabaseIntegration
{
    public interface IDatabaseIntegration
    {
        /// <summary>
        /// Discover all ids listed in the view (all rows)
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        IEnumerable<string> DiscoverInitialIds(SqlDatabaseConnectorJobConfiguration config);

        /// <summary>
        /// Discover all ids that has been changed in the main View
        /// </summary>
        /// <param name="config"></param>
        /// <param name="sourceChanges"></param>
        /// <returns></returns>
        IEnumerable<string> DiscoverIncrementalIds(SqlDatabaseConnectorJobConfiguration config, ref DatabaseJobState DatabaseJobState , EventType changeType);

        IEnumerable<string> DiscoverMainTableIds(SqlDatabaseConnectorJobConfiguration config, string constraintColumn, IList<string> foreignIds);
        
        /// <summary>
        /// Fetches documents with the corresponding Ids
        /// </summary>
        /// <param name="constraintColumn"></param>
        /// <param name="idsToSelect"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        IEnumerable<AddDocument> FetchDocuments(SqlDatabaseConnectorJobConfiguration config, string constraintColumn, bool constraintIsIntType, IEnumerable<string> idsToSelect);

        long GetLastChangeVersion(SqlDatabaseConnectorJobConfiguration config, TableDetail changeTableSettings);
        long GetLastChangeVersion(SqlDatabaseConnectorJobConfiguration config, EventTable eventTableSettings);



    }


}
