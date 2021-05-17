using System;
using System.Collections.Generic;
using System.Threading;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain.Jobs;
using Bifrost.Common.QuartzIntegration.Jobs;
using Bifrost.Common.QuartzIntegration.Jobs.Internal;
using Bifrost.Common.QuartzIntegration.Server;
using Bifrost.Common.Tests.TestDomain.Jobs;
using FluentAssertions;
using Moq;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Xunit;

namespace Bifrost.Common.Tests.QuartzTests
{
    public class UpdateInMemoryJobTests
    {


        [Fact]
        public void WillExecuteAtServerInitialization()
        {
            var scheduleFactory = new StdSchedulerFactory();
            var getJobCalled = false;
            var mockedJobService = new Mock<IJobService>();
            mockedJobService
                .Setup(x=>x.Get())
                .Callback(() => getJobCalled = true)
                .Returns(new List<Job>() {new Job() {Configuration = new SimpleConfig()} });
            var updateJob = new UpdateInMemoryQuartzJob(new IConfigurableJob[] { new SimpleJob(mockedJobService.Object), }, mockedJobService.Object,scheduleFactory );

            var mockedFactory = new Mock<IJobFactory>(MockBehavior.Default);
            mockedFactory
                .Setup(x => x.NewJob(It.IsAny<TriggerFiredBundle>(), It.IsAny<IScheduler>()))
                .Returns(updateJob);

            var server = new QuartzServer(mockedFactory.Object, new GlobalJobListener(), scheduleFactory, new [] {updateJob});
            server.Initialize();

            getJobCalled.Should().BeTrue();
        }


        [Fact]
        public void WillScheduleJob()
        {
            var getJobCalled = false;
            var schedulerCalled = false;
            var scheduleFactory = new Mock<ISchedulerFactory>();
            var scheduler = new StdSchedulerFactory().GetScheduler();
            scheduleFactory.Setup(x => x.GetScheduler(It.IsAny<CancellationToken>()))
                .Callback(() => schedulerCalled = true)
                .Returns(scheduler);
            
            var mockedJobService = new Mock<IJobService>();
            mockedJobService
                .Setup(x => x.Get())
                .Callback(() => getJobCalled = true)
                .Returns(new List<Job>() { new Job() { Configuration = new SimpleConfig(), LastUpdated = DateTime.Now, Name = "test", TriggerCronSyntax = "*/4 * * * * ?" } });
            var updateJob = new UpdateInMemoryQuartzJob(
                new IConfigurableJob[] { new SimpleJob(mockedJobService.Object), },
                mockedJobService.Object, scheduleFactory.Object);

            var mockedFactory = new Mock<IJobFactory>(MockBehavior.Default);
            mockedFactory
                .Setup(x => x.NewJob(It.IsAny<TriggerFiredBundle>(), It.IsAny<IScheduler>()))
                .Returns(updateJob);

            var server = new QuartzServer(mockedFactory.Object, new GlobalJobListener(), scheduleFactory.Object, new[] { updateJob });
            server.Initialize();

            getJobCalled.Should().BeTrue();
            schedulerCalled.Should().BeTrue();
        }
    }
}
