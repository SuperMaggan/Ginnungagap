using System;
using System.Security;
using Bifrost.ClientAPI.Infrastructure;
using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Domain;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.ApplicationServices.Implementations;
using Serilog;

namespace Bifrost.ClientAPI.Controllers.Services
{
  public class SecurityService : ISecurityService
  {


    private readonly ICache<string, Consumer> _consumerCache;
    private readonly ClientApiSettings _settings;
    private readonly IConsumerService _consumerService;

    public SecurityService(ClientApiSettings settings, IConsumerService consumerService)
    {
      _settings = settings;
      _consumerService = consumerService;
      _consumerCache = new TimedMemoryCache<string, Consumer>(_consumerService.GetConsumerByApiKey, new TimeSpan(0, 0, 0, 0, _settings.CacheInMS));
    }

    public Consumer GetConsumer(string apiKey)
    {
      var consumer = _consumerCache.Retrieve(apiKey);
      if (consumer == null)
      {
        Log.Logger.Warning($"Someone tried to access a function with an invalid apikey: {apiKey}");
        throw new SecurityException($"No consumer with apikey {apiKey} exists. Begone!");
      }
      return consumer;
    }
  }
}