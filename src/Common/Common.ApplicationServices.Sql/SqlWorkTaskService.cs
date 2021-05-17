using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Bifrost.Common.ApplicationServices.Sql.Common;
using Bifrost.Common.ApplicationServices.Sql.Common.Extensions;
using Bifrost.Common.ApplicationServices.Sql.Databases;
using Bifrost.Common.ApplicationServices.Sql.Models;
using Bifrost.Common.ApplicationServices.Sql.Models.Mappers;
using Bifrost.Common.ApplicationServices.Sql.Tables;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain;
using Serilog;

namespace Bifrost.Common.ApplicationServices.Sql
{
    public class SqlWorkTaskService : IWorkTaskService
    {
        private readonly DatabaseBase _database;

        public SqlWorkTaskService(WorkTaskDatabase database)
        {
            _database = database;
            database.SetupOnce();
        }


        /// <summary>
        ///     Returns the next (if available) Task. The task will be "checked out" and needs to be
        ///     "checked in" when done with
        /// </summary>
        /// <param name="owner">Name of the owning job/category whose task to get</param>
        /// <returns>Null if no task for this owner exist</returns>
        public WorkTask GetNextTask(string owner)
        {
            using (var connection = _database.GetOpenConnection())
            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                var model = connection.Query<WorkTaskModel>(
                    string.Format("UPDATE TOP(1) {0} SET CheckedOut=1 OUTPUT DELETED.* WHERE Owner=@owner AND CheckedOut=0",
                        WorkTaskTable.TableName),
                    new {owner}, transaction).FirstOrDefault();
                if (model == null)
                    return null;
                transaction.Commit();
                return model.ToDomain();
            }
        }

        /// <summary>
        ///     Considers the Task completed and deletes it
        /// </summary>
        public void TaskCompleted(WorkTask workTask)
        {
            using (var connection = _database.GetOpenConnection())
            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                var model = workTask.ToModel();
                var deleted = connection.Execute(
                    string.Format("DELETE FROM {0} WHERE Owner=@owner AND Id=@id AND CheckedOut=1",
                        WorkTaskTable.TableName),
                    new {owner = model.Owner, id = model.Id},
                    transaction);
                if (deleted == 0)
                    Log.Warning(
                        string.Format("Tried to mark a WorkTask (id={0}) as completed when that task didn't exist",
                            model.Id));

                transaction.Commit();
            }
        }

        public void RevertTask(WorkTask workTask)
        {
            using (var connection = _database.GetOpenConnection())
            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                var model = workTask.ToModel(false);
                connection.Update(model, transaction);
                transaction.Commit();
            }
        }

        /// <summary>
        ///     Serializes the given values into tasks
        ///     Will delete any existing tasks
        /// </summary>
        /// <param name="owner">>Name of the owning job/category that the task should belong to</param>
        /// <param name="taskSize">number of values each task should contain</param>
        /// <param name="values"></param>
        /// <returns>Number of batch files created</returns>
        public int WriteTasks(string owner, int taskSize, IList<string> values)
        {
            DeleteAllTasks(owner);

            var numBatches = (values.Count/taskSize) + ((values.Count%taskSize) > 0 ? 1 : 0);
            using (var connection = _database.GetOpenConnection())
            {
                for (var factor = 0; factor < numBatches; factor++)
                {
                    var start = factor*taskSize;
                    var numElements = (values.Count >= (start + taskSize)) ? (taskSize) : (values.Count - start);
                    var model = new WorkTaskModel
                    {
                        CheckedOut = false,
                        Owner = owner,
                        InstructionsCsv = TakeSubArray(values, start, numElements).ToCsvString()
                    };
                    connection.Insert(model);
                }
            }
            return numBatches;
        }

        /// <summary>
        ///     Serializes the given values into one task
        ///     Will not remove the existing tasks
        /// </summary>
        /// <param name="owner">>Name of the owning job/category that the batch should belong to</param>
        /// <param name="values"></param>
        public void WriteTasks(string owner, IList<string> values)
        {
            using (var connection = _database.GetOpenConnection())
            {
                var model = new WorkTaskModel
                {
                    CheckedOut = false,
                    Owner = owner,
                    InstructionsCsv = values.ToCsvString()
                };
                connection.Insert(model);
            }
        }

        /// <summary>
        ///     Deletes all tasks
        /// </summary>
        public void DeleteAllTasks(string owner)
        {
            using (var connection = _database.GetOpenConnection())
            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                connection.Execute(
                    $"DELETE FROM {WorkTaskTable.TableName} WHERE Owner=@owner",
                    new {owner},
                    transaction);
                transaction.Commit();
            }
        }

        private IEnumerable<string> TakeSubArray(IList<string> array, int start, int count)
        {
            var end = start + count;
            for (var i = start; i < end; i++)
            {
                yield return array[i];
            }
        }
    }
}