using Bifrost.Core.Domain;

namespace Bifrost.Connector.SqlDatabase.DatabaseConnectorState.ChangeTracking
{
    public class ChangeTableEvent
    {
        public ChangeTableEvent(long version, string primaryKeyId, EventType eventType)
        {
            EventType = eventType;
            PrimaryKeyId = primaryKeyId;
            Version = version;
        
        }

        public long Version { get; }
        public string PrimaryKeyId { get; }
        public EventType EventType { get; }

    }
}

