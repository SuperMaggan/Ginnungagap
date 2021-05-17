using System;
using System.Collections.Generic;
using Bifrost.Core.Connectors.Configs.SqlDatabase;
using Bifrost.Core.Domain;

namespace Bifrost.Connector.SqlDatabase.DatabaseConnectorState.ChangeTracking
{
    public class DataChange
    {
        public DataChange(TableDetail table)
        {
            this.Table = table;
            AddedIds = new List<string>();
            DeletedIds = new List<string>();
        }

        /// <summary>
        /// Inclusive
        /// </summary>
        public long HighestVersionAdded { get; set; }
        public TableDetail Table{ get; set; }
        public IList<string> AddedIds { get; set; }
        public IList<string> DeletedIds { get; set; }

        public void AddChange(ChangeTableEvent dataRowToChangeTableEvent)
        {
            if(dataRowToChangeTableEvent.EventType == EventType.Add)
                AddedIds.Add(dataRowToChangeTableEvent.PrimaryKeyId);
            else if (dataRowToChangeTableEvent.EventType == EventType.Delete)
                DeletedIds.Add(dataRowToChangeTableEvent.PrimaryKeyId);
            else
                throw new NotSupportedException(
                    $"Datachange of type {dataRowToChangeTableEvent.EventType} is not supported");

            if (HighestVersionAdded < dataRowToChangeTableEvent.Version)
                HighestVersionAdded = dataRowToChangeTableEvent.Version;
        }


    }
}
