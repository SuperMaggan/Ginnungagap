using Bifrost.Common.Core.Settings;

namespace Bifrost.Common.ApplicationServices.Sql.Common
{
    public class CommonDatabaseSettings : ConfigSettingsBase
    {
        /// <summary>
        ///     The connection that Database service implementations uses
        /// </summary>
        public ConnectionStringSettings CommonConnection { get; set; }

        /// <summary>
        ///     If true at startup, all Common database objects will be deleted before created
        ///     (Database will be cleared before startup)
        /// </summary>
        public bool DestroyCommonDatabase { get; set; }

        public bool AutomaticallyCreateDatabase { get; set; }
    }
}