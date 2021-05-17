using Bifrost.Core.Settings;
using Bifrost.Common.Core.Settings;

namespace Bifrost.IntegrationTests.Connectors.SqlDatabase.TestDatabaseObjects
{
    public class TestSettings : AsgardSettingsBase
    {
        public ConnectionStringSettings TestSqlDatabaseConnection { get; set; }
    }
}