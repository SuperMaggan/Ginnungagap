using System;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.IntegrationTests.Utilities;
using FluentAssertions;

namespace Bifrost.Common.IntegrationTests.DatabaseTests
{
    public abstract class WorkTaskServiceTest
    {
        protected abstract IWorkTaskService CreateService();

        /// <summary>
        ///     Clear all persisted items for the given owner/name
        /// </summary>
        protected abstract void Clear(string name);

        protected void CanInitialize()
        {
            Action action = () => CreateService();
            action.Should().NotThrow();
        }

        public virtual void CanWriteBatchedTasks()
        {
            var service = CreateService();

            var name = "CanWriteBatchedTasks";
            var batchSize = 100;
            var numberOfIds = 1005;
            var ids = TestDataUtility.GenerateRandomStringList(numberOfIds, 20);
            var taskCount = service.WriteTasks(name, batchSize, ids);
            var expectedCount = (numberOfIds/batchSize) + (numberOfIds%batchSize > 0 ? 1 : 0);
            taskCount.Should().Be(expectedCount);
            Clear(name);
        }

        public virtual void CanWriteNonBatchedTasks()
        {
            var service = CreateService();
            var name = "CanWriteNonBatchedTasks";
            var numberOfIds = 110;
            var ids = TestDataUtility.GenerateRandomStringList(numberOfIds, 20);

            var shouldBeNull = service.GetNextTask(name);
            shouldBeNull.Should().BeNull("No tasks should exist yet");

            service.WriteTasks(name, ids);
            var task = service.GetNextTask(name);
            task.Should().NotBeNull("One task should exist");
            task.GroupName.Should().Be(name);
            task.Instructions.Should().BeEquivalentTo(ids);

            shouldBeNull = service.GetNextTask(name);
            shouldBeNull.Should().BeNull("No more tasks should exist");
            Clear(name);
        }

        public virtual void CanGetNextTask()
        {
            var service = CreateService();
            var name = "CanGetNextTask";
            var batchSize = 100;
            var numberOfIds = 200;
            var ids = TestDataUtility.GenerateRandomStringList(numberOfIds, 20);
            var taskCount = service.WriteTasks(name, batchSize, ids);
            taskCount.Should().Be(2);

            var firstTask = service.GetNextTask(name);
            var secondTask = service.GetNextTask(name);
            var thirdTask = service.GetNextTask(name);
            firstTask.Should().NotBeNull();
            secondTask.Should().NotBeNull();
            thirdTask.Should().BeNull();

            firstTask.Instructions.Should().HaveCount(batchSize);
            secondTask.Instructions.Should().HaveCount(batchSize);
            Clear(name);
        }

        public virtual void CanCompleteTasks()
        {
            var service = CreateService();
            var name = "CanCompleteTasks";
            var batchSize = 100;
            var numberOfIds = 200;
            var ids = TestDataUtility.GenerateRandomStringList(numberOfIds, 20);
            var taskCount = service.WriteTasks(name, batchSize, ids);
            taskCount.Should().Be(2);

            var firstTask = service.GetNextTask(name);
            var secondTask = service.GetNextTask(name);
            firstTask.Should().NotBeNull();
            secondTask.Should().NotBeNull();

            firstTask.Instructions.Should().HaveCount(batchSize);
            secondTask.Instructions.Should().HaveCount(batchSize);

            service.TaskCompleted(firstTask);
            service.TaskCompleted(secondTask);
            Clear(name);
        }

        public virtual void CanDeleteAllTasks()
        {
            var service = CreateService();

            var name = "CanDeleteAllTasks";
            var batchSize = 100;
            var numberOfIds = 1005;
            var ids = TestDataUtility.GenerateRandomStringList(numberOfIds, 20);
            var taskCount = service.WriteTasks(name, batchSize, ids);
            var expectedCount = (numberOfIds/batchSize) + (numberOfIds%batchSize > 0 ? 1 : 0);
            taskCount.Should().Be(expectedCount);

            service.DeleteAllTasks(name);
            var shouldBeNull = service.GetNextTask(name);
            shouldBeNull.Should().BeNull("All tasks should've been deleted");
            Clear(name);
        }
    }
}