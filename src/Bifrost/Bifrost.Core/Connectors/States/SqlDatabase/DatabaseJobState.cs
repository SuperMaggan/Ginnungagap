using System;
using System.Globalization;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Core.Connectors.States.SqlDatabase
{
    public class DatabaseJobState: ConnectorStateBase
    {
        public DatabaseJobState()
        {
        }

        public DatabaseJobState(State state)
        {
            Name = state.Name;
            Fields = state.Fields;
        }
        public int DiscoverCount { get; set; }
        public DateTime? LastDiscoverDate { get; set; }

        //Number of batches that was found the last discover round
        public int LastDiscoverBatchCount { get; set; }

        private string GetChangeTableFieldName(string changeTableName)
        {
            return string.Format("Changetable_{0}", changeTableName);
        }
        public long GetChangetableVersionThreshold(string changeTableName)
        {
            long version;
            var fieldValue = GetValue(GetChangeTableFieldName(changeTableName));
            if(fieldValue == null){ //this must be a new changetable
                SetChangetableVersionThreshold(changeTableName,0);
                return 0;
            }
            if (!long.TryParse(fieldValue, out version))
                throw new MissingFieldException(String.Format("{0} has changetable threshold set to {1} which is an invalid value (need to be number)"
                    , changeTableName, fieldValue));

            return version;
        }

        public void SetChangetableVersionThreshold(string changeTableName, long versionThreshold, bool onlySetIfGreater = false)
        {
            if (!onlySetIfGreater) { 
                SetValue(GetChangeTableFieldName(changeTableName), versionThreshold.ToString(CultureInfo.InvariantCulture));
                return;
            }
            try
            {
                var orgValue = GetChangetableVersionThreshold(GetChangeTableFieldName(changeTableName));
                if (orgValue < versionThreshold)
                    SetValue(GetChangeTableFieldName(changeTableName), versionThreshold.ToString(CultureInfo.InvariantCulture));
            }
            catch (MissingFieldException)
            {
                SetValue(GetChangeTableFieldName(changeTableName), versionThreshold.ToString(CultureInfo.InvariantCulture));
            }

        }

       

    }
}