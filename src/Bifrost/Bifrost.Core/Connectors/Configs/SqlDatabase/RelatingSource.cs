using System.Collections.Generic;

namespace Bifrost.Core.Connectors.Configs.SqlDatabase
{
    //[Serializable]
    public class RelatingSource
    {

        public RelatingSource()
        {
            RelationIsInteger = true;
        }
        /// <summary>
        /// Describes how the source is related, what key that binds them togheter
        /// MainSourceField -> SubSourceName
        /// </summary>
        public KeyValuePair<string,string> Relation { get; set; }
        public bool RelationIsInteger { get; set; }
        public SqlDatabaseConnectorJobConfiguration SqlDatabaseConnectorConfiguration { get; set; }
    }

 
}