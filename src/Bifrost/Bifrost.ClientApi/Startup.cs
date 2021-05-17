using System;
using System.IO;
using System.Linq;
using System.Threading;
using Bifrost.ClientAPI.Controllers.Services;
using Bifrost.ClientAPI.Infrastructure;
using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Domain;
using Bifrost.Core.Infrastructure;
using Bifrost.Data.Sql.Infrastructure;
using Bifrost.Common.Core.ApplicationServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace Bifrost.ClientAPI
{
  public class Startup
  {
    public IConfigurationRoot Configuration { get; }
    private Timer _processInformationUpdater;

    public Startup(IWebHostEnvironment env)
    {
      var builder = new ConfigurationBuilder()
          .SetBasePath(env.ContentRootPath)
          .AddEnvironmentVariables();
      Configuration = builder.Build();
      Log.Logger = BifrostConfiguration.ConfigureLogger();
    }

    // ConfigureServices is where you register dependencies. This gets
    // called by the runtime before the ConfigureContainer method, below.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers();

      // Register the Swagger generator, defining one or more Swagger documents
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo{ Title = "Bifrost api", Version = "v1" });
        if (File.Exists("Bifrost.ClientAPI.xml"))
          c.IncludeXmlComments("Bifrost.ClientAPI.xml");
      });
      services.AddCors();

    

    }

    // ConfigureContainer is where you can register things directly
    // with Autofac. This runs after ConfigureServices so the things
    // here will override registrations made in ConfigureServices.
    // Don't build the container; that gets done for you by the factory.
    public void ConfigureContainer(ContainerBuilder builder)
    {

      builder.RegisterModule<BifrostSqlDataModule>();
      builder.RegisterModule<JobConfigurationModule>();
      builder.RegisterType<ClientApiSettings>().AsSelf().SingleInstance();
      builder.RegisterType<SecurityService>().As<ISecurityService>().SingleInstance();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
      loggerFactory.AddSerilog(dispose: true);
      app.UseMiddleware<SerilogMiddleware>();

      app.UseSwagger();

      // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Asgard admin API v1");
      });
      app.UseRouting();

      var setting = app.ApplicationServices.GetService<ClientApiSettings>();
      if (!string.IsNullOrEmpty(setting?.AllowedCorsOriginsCsv))
      {
          app.UseCors(opt =>
            {
              foreach (var origin in setting.AllowedCorsOriginsCsv.Split(';', ','))
              {
                opt.AllowAnyHeader();
                opt.AllowAnyMethod();
                opt.WithOrigins(origin);
              }
            }
        );
      }
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });

      var autofacContainer = app.ApplicationServices.GetAutofacRoot();
      var processInfoService = autofacContainer.Resolve<IProcessInformationService>();
      if (processInfoService != null)
        _processInformationUpdater = new Timer(state =>
        {
          processInfoService.SaveCurrent("Hades.ClientApi");
        }, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(processInfoService.CreateSnapshot("Bifrost.ClientApi").UpdateFrequencySec));

      var consumerService = autofacContainer.Resolve<IConsumerService>();
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
