namespace Bifrost.Core.Connectors.Configs.SqlDatabase
{
    //    [Serializable]
    public class EventTable
    {
        /// <summary>
        /// Name of the table holding the events
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Name of the column holding the sequence number of the event
        /// This is typically an incremental identity 
        /// </summary>
        public string EventSequenceColumnName { get; set; }

        /// <summary>
        /// Name of the column holding the id that the event is referring to
        /// </summary>
        public string MainTableIdColumnName { get; set; }

        /// <summary>
        /// Name of the column holding information about what type of event it is
        /// </summary>
        public string EventTypeColumnName { get; set; }

        /// <summary>
        /// What is a delete event type called.
        /// Note that we only need the delete since all other events are treated as ADD
        /// </summary>
        public string DeleteEventTypeValue { get; set; }
    }
}