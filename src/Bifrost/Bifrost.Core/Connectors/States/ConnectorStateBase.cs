using System;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Core.Connectors.States
{
    public class ConnectorStateBase : Common.Core.Domain.Jobs.JobState
    {
        public ConnectorStateBase()
        {
        }

        public ConnectorStateBase(State state)
        {
            Name = state.Name;
            Fields = state.Fields;
            ConnectorStateType = this.GetType().Name;
        }

        /// <summary>
        /// Current state
        /// </summary>
        public JobState State { get; set; }

        /// <summary>
        /// Previous working state
        /// </summary>
        public JobState LastWorkingState { get; set; }

        /// <summary>
        /// When the job was initialized (setup)
        /// </summary>
        public DateTime? InitDate{ get; set; }
        
       
        public int BatchCount { get; set; }

        /// <summary>
        /// Type of ConnectorState class, used for deserializing it to the correct type
        /// </summary>
        public string ConnectorStateType { get; set; }
    }
}