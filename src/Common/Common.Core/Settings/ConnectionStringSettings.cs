namespace Bifrost.Common.Core.Settings
{
    /// <summary>
    /// USed since ConfigurationManager still nt exists in netstandard 1.6
    /// Remove once 2.0 has been release
    /// </summary>
    public class ConnectionStringSettings
    {
        public string ConnectionString { get; set; }
        public string ProviderName { get; set; }

        public override string ToString()
        {
            return ConnectionString;
        }
    }
}