using System.Collections.Generic;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Common.Core.ApplicationServices
{
    public interface IWorkTaskService
    {
        /// <summary>
        ///     Returns the next (if available) Task. The task will be "checked out" and needs to be
        ///     "checked in" when done with
        /// </summary>
        /// <param name="groupName">Name of the owning job/category whose task to get</param>
        /// <returns>Null if no task for this owner exist</returns>
        WorkTask GetNextTask(string groupName);

        /// <summary>
        ///     Marks the given task as completed
        /// </summary>
        void TaskCompleted(WorkTask workTask);

        /// <summary>
        ///     Revert th egiven task to its original state, making it possible for another process to handle it
        /// </summary>
        /// <param name="workTask"></param>
        void RevertTask(WorkTask workTask);

        /// <summary>
        ///     Serializes the given values into tasks
        ///     Will delete any existing tasks
        /// </summary>
        /// <param name="groupName">>Name of the owning job/category that the task should belong to</param>
        /// <param name="taskSize">number of values each task should contain</param>
        /// <param name="instructions"></param>
        /// <returns>Number of batch files created</returns>
        int WriteTasks(string groupName, int taskSize, IList<string> instructions);

        /// <summary>
        ///     Serializes the given values into one task
        ///     Will not remove the existing tasks
        /// </summary>
        /// <param name="owner">>Name of the owning job/category that the batch should belong to</param>
        /// <param name="instructions"></param>
        void WriteTasks(string owner, IList<string> instructions);

        /// <summary>
        ///     Deletes all tasks belonging to the given group
        /// </summary>
        void DeleteAllTasks(string groupName);
    }
}