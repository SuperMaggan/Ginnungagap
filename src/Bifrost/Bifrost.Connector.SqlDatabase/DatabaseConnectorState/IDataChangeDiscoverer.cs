using System.Collections.Generic;
using Bifrost.Connector.SqlDatabase.DatabaseConnectorState.ChangeTracking;
using Bifrost.Core.Connectors.Configs;
using Bifrost.Core.Connectors.States.SqlDatabase;

namespace Bifrost.Connector.SqlDatabase.DatabaseConnectorState
{
    /// <summary>
    /// Handles how change in a table is detected
    /// </summary>
    public interface IDataChangeDiscoverer
    {
        IEnumerable<DataChange> DiscoverChanges(
            SqlDatabaseConnectorJobConfiguration config,
            DatabaseJobState stateBase);

        bool CanHandle(SqlDatabaseConnectorJobConfiguration config);
    }
}