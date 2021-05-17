using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Domain;
using Serilog;
using Serilog.Events;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Bifrost.ClientAPI.Infrastructure
{
  public static class BifrostConfiguration
  {
    public static ILogger ConfigureLogger()
    {
      return new LoggerConfiguration()
                   .Enrich.FromLogContext()
                   .WriteTo.LiterateConsole()
                   .WriteTo.RollingFile("data/logs/log-{Date}.txt")
                   .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                   .MinimumLevel.Override("System", LogEventLevel.Warning)
                   .CreateLogger();
    }

    public static void SetupDefaultUser(IServiceProvider provider)
    {
      var consumerService = provider.GetService<IConsumerService>();
      if (!consumerService.GetAllConsumers().Any(x => x.IsAdmin))
      {
        consumerService.SaveOrUpdate(new Consumer()
        {
          Name = "SuperAdmin",
          ApiKey = "Kite4Life",
          IsAdmin = true
        });
      }
    }
  }
}
