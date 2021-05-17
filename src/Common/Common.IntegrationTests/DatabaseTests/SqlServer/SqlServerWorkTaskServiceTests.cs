using Bifrost.Common.ApplicationServices.Sql;
using Bifrost.Common.ApplicationServices.Sql.Common;
using Bifrost.Common.ApplicationServices.Sql.Databases;
using Bifrost.Common.Core.ApplicationServices;
using Xunit;

namespace Bifrost.Common.IntegrationTests.DatabaseTests.SqlServer
{
    public class SqlServerWorkTaskServiceTests : WorkTaskServiceTest
    {
        protected override IWorkTaskService CreateService()
        {
            var settings = new CommonDatabaseSettings();
            var database = new WorkTaskDatabase(settings);

            return new SqlWorkTaskService(database);
        }

        /// <summary>
        ///     Clear all persisted items
        /// </summary>
        protected override void Clear(string name)
        {
            CreateService().DeleteAllTasks(name);
        }

        [Fact]
        public override void CanCompleteTasks()
        {
            base.CanCompleteTasks();
        }

        [Fact]
        public override void CanDeleteAllTasks()
        {
            base.CanDeleteAllTasks();
        }

        [Fact]
        public override void CanGetNextTask()
        {
            base.CanGetNextTask();
        }

        [Fact]
        public override void CanWriteBatchedTasks()
        {
            base.CanWriteBatchedTasks();
        }

        [Fact]
        public override void CanWriteNonBatchedTasks()
        {
            base.CanWriteNonBatchedTasks();
        }
    }
}