using System.Linq;
using Bifrost.Core.Settings;
using Bifrost.Common.Core.Settings;
using Serilog;

namespace Bifrost.Data.Sql
{
    public class SqlSettings : AsgardSettingsBase
    {
        public ConnectionStringSettings BifrostConnection { get; set; }
        public bool DestroyBifrostDatabase { get; set; }

        /// <summary>
        /// If db not existing, try create it
        /// </summary>
        public bool AutomaticallyCreateDatabase { get; set; }
    }
}
