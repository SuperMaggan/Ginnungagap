using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Bifrost.Core.Connectors;
using Bifrost.Core.Connectors.Configs;
using Bifrost.Core.Connectors.Configs.Common;
using Bifrost.Core.Connectors.Configs.SqlDatabase;
using Bifrost.Core.Connectors.Configs.Web;
using Bifrost.Core.Domain;
using Bifrost.JobsRunner.Infrastructure;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain.Jobs;
using Bifrost.Common.QuartzIntegration.Server;
using Serilog;
using Serilog.Core;
using Autofac;

namespace Bifrost.JobsRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .WriteTo.RollingFile("data/log-{Date}.txt")
                .MinimumLevel.Debug()
                .CreateLogger();

            var container = Bootstrapper.CreateContainer();
                var quartzServer = container.Resolve<IQuartzServer>();
            quartzServer.Initialize();
            quartzServer.Start();
         
            while (true)
            {
                Log.Logger.Information("Server running..");
                Thread.Sleep(10000);
            }

        }




        private static void CreateCrawlBlogWebJob(IContainer container)
        {
            var config = new WebConnectorJobConfiguration()
            {
                StartUrl = "http://blog.cwa.me.uk/",
                Credential = null,
                DefaultVerifyFrequency = new Frequency() { Minutes = 2 },
                Depth = 2,
                JobName = "TestWebJob",
                NumberOfPagesPerExecution = 10
            };
            var webJob = new Job()
            {
                ConcurrentLimit = 1,
                Configuration = config,
                Description = "Testar",
                Enabled = true,
                Name = "TestWebJob",
                TriggerCronSyntax = "*/10 * * * * ?"
            };
            container.Resolve<IJobService>().SaveOrUpdate(webJob);

        }


        private static void CreateFileSystemJob(IContainer container)
        {
            var config = new WebConnectorJobConfiguration()
            {
                StartUrl = "https://www.taqaglobal.com/investors/h1-2017-financial-results",
                Credential = null,
                DefaultVerifyFrequency = new Frequency() { Minutes = 2 },
                Depth = 1,
                JobName = "FileSystemJob",
                NumberOfPagesPerExecution = 10,
                PageFilter = new PageFilter() { ExcludeBinaryPages = false}
            };
            var webJob = new Job()
            {
                ConcurrentLimit = 1,
                Configuration = config,
                Description = "Testar",
                Enabled = true,
                Name = "FileSystemJob",
                TriggerCronSyntax = "*/10 * * * * ?"
            };
            container.Resolve<IJobService>().SaveOrUpdate(webJob);

        }
    }
}