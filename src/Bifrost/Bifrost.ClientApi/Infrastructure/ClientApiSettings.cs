using Bifrost.Core.Settings;

namespace Bifrost.ClientAPI.Infrastructure
{
  public class ClientApiSettings : AsgardSettingsBase
  {
    /// <summary>
    /// How long to cache different things, eg, Consumer API keys and domain names
    /// </summary>
    public int CacheInMS { get; set; } = 10000;
    public string AllowedCorsOriginsCsv { get; set; }
  }
}