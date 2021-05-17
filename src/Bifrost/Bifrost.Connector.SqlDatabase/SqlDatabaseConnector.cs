using System;
using System.Collections.Generic;
using System.Linq;
using Bifrost.Connector.SqlDatabase.DatabaseIntegration;
using Bifrost.Core.Connectors;
using Bifrost.Core.Connectors.Configs;
using Bifrost.Core.Connectors.States.SqlDatabase;
using Bifrost.Core.Domain;
using Bifrost.Core.Utils;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain;
using Bifrost.Common.Core.Domain.Jobs;
using Serilog;
using JobState = Bifrost.Core.Connectors.States;
namespace Bifrost.Connector.SqlDatabase
{
    public class SqlDatabaseConnector : ISqlDatabaseConnector
    {
        private readonly IWorkTaskService _workTaskService;
        private readonly IStateService _stateService;
        private readonly IDictionary<string, IDatabaseIntegration> _databaseIntegrations;

        //For how many job executions a job can be in Discover mode. Used if the job has been stuck (process terminated)
        private const int ErrorCountLimit = 20;
        
        public SqlDatabaseConnector(
            IWorkTaskService workTaskService,
            IStateService stateService,
            IDatabaseIntegration[] databaseIntegrations
            )
        {
            _workTaskService = workTaskService;
            _stateService = stateService;
            _databaseIntegrations = databaseIntegrations.ToDictionary(x=>x.GetType().Name);
        }

        public void ResetConnector(string jobName)
        {
            Log.Information($"Resetting state for job {jobName}");
            var state = _stateService.LoadState(jobName);
            if (state == null)
                return;
            var jobState = new DatabaseJobState(state) {State = JobState.JobState.Paused};
            _stateService.SaveState(jobState);
            _workTaskService.DeleteAllTasks(jobState.Name);
            _stateService.DeleteState(jobState.Name);
        }

        public SourceChanges ExecuteFetch(SqlDatabaseConnectorJobConfiguration config)
        {
            try
            {
                if(config.ChangeTables.Any() && config.EventTables.Any())
                    throw new Exception("Cannot handle both change tables and event tables at the same time");
                var jobState = GetCurrentJobState(config);
                jobState.LastExecutionDate = DateTime.UtcNow;
                var changes = HandleState(jobState, config);
                return changes;
            }
            catch(Exception e)
            {
                Log.Logger.Error(e,$"{config.JobName}: error while executing fetch. Setting job to error state");
                SetErrorState(config, e);
                throw;
            }
        }


        private SourceChanges HandleState(DatabaseJobState jobStateBase, SqlDatabaseConnectorJobConfiguration configuration)
        {
            switch (jobStateBase.State)
            {
                case JobState.JobState.Paused:
                    return new SourceChanges();
                case JobState.JobState.InitialCrawling:
                    return HandleInitialCrawlingState(jobStateBase, configuration);
                case JobState.JobState.Discovering:
                    return HandleDiscoveringState(jobStateBase);
                case JobState.JobState.Error:
                    return HandleErrorState(jobStateBase,configuration);
                case JobState.JobState.IncrementalCrawling:
                    return HandleIncrementalCrawling(configuration, jobStateBase);
                default:
                    throw new NotSupportedException($"{jobStateBase.Name}: State is in a non-supported state: {jobStateBase.State}.");
            }
        }

        /// <summary>
        /// Will set the state to Discover and start to discover all document ids
        /// the Ids will be serialized to batch files
        /// </summary>
        /// <param name="jobStateBase"></param>
        /// <param name="config"></param>
        private SourceChanges HandleInitialCrawlingState(DatabaseJobState jobStateBase, SqlDatabaseConnectorJobConfiguration config)
        {
            jobStateBase.State = JobState.JobState.Discovering;
            jobStateBase.Status = JobStatus.Ok;
            _stateService.SaveState(jobStateBase);
            try
            {
                //Discover all ids
                var discovedIds = GetIntegration(config).DiscoverInitialIds(config);
                //Serialize them to batches
                var numBatches =_workTaskService.WriteTasks(config.JobName, config.BatchSize, discovedIds.ToList());
                jobStateBase.BatchCount = numBatches;
                jobStateBase.State = JobState.JobState.IncrementalCrawling;
                jobStateBase.LastDiscoverBatchCount = numBatches;
                _stateService.SaveState(jobStateBase);
                return HandleIncrementalCrawling(config, jobStateBase);
            }
            catch(Exception e)
            {
                jobStateBase.SetErrorState(e);
                _stateService.SaveState(jobStateBase);
                throw;
            }
        }

        /// <summary>
        /// If the current state is in Discovering that means another worker is currently trying to discover
        /// if there are any changes in the source.
        /// Another option is that the previous worker unexpectively crashed. To repair such scenario
        /// a state can only be in Discovering phase for 30 iterations before being reset to IncrementalCrawl
        /// </summary>
        /// <param name="jobStateBase"></param>
        private SourceChanges HandleDiscoveringState(DatabaseJobState jobStateBase)
        {
            if (ErrorCountLimit - jobStateBase.DiscoverCount > 0)
            {
                Log.Logger.Information($"{jobStateBase.Name}: State in DISCOVER phase (for {jobStateBase.DiscoverCount} job executions. Will reset to IncrementalCrawl in {ErrorCountLimit - jobStateBase.DiscoverCount} executions");
                jobStateBase.DiscoverCount++;
            }
            else
            {
                Log.Logger.Warning($"{jobStateBase.Name}: State in DISCOVER phase, will reset to IncrementalCrawling");
                jobStateBase.State = JobState.JobState.IncrementalCrawling;
                jobStateBase.DiscoverCount = 0;
            }
            jobStateBase.Status = JobStatus.Ok;
            _stateService.SaveState(jobStateBase);
            return new SourceChanges();
        }

        private SourceChanges HandleIncrementalCrawling(SqlDatabaseConnectorJobConfiguration configuration, DatabaseJobState jobStateBase)
        {
            //try to get the first file of batch ids
            var workTask = _workTaskService.GetNextTask(configuration.JobName);

            if (workTask!= null)
            {
                Log.Logger.Information(
                    $"{jobStateBase.Name}: State in INCREMENTAL CRAWLING phase (still {jobStateBase.BatchCount} batches left)");
                if (jobStateBase.BatchCount > 0)
                    jobStateBase.BatchCount --;
                jobStateBase.Status = JobStatus.Ok;
                _stateService.SaveState(jobStateBase);
                return FetchDocumentsUsingWorkTask(configuration, workTask);
            }
            if(ResetIfConfigured(configuration,jobStateBase))
                return new SourceChanges();

            return FetchDocumentsUsingIncremental(configuration, jobStateBase);
        }


        private bool ResetIfConfigured(SqlDatabaseConnectorJobConfiguration config, DatabaseJobState jobStateBase)
        {
            if(config.ResetEveryXHour <= 0)
                return false;
            var hoursSinceLastCrawl = (DateTime.UtcNow - jobStateBase.InitDate.Value).TotalHours;
            if (hoursSinceLastCrawl >= config.ResetEveryXHour) { 
                ResetConnector(config.JobName);
                return true;
            }
            return false;
        }


        /// <summary>
        /// If the job currently is in an Error state it will be reset to its previously working state
        /// With an exception. If the previously state was Discover mode it will be reset to IncrementalCrawling
        /// Why? Because there might be a few document batches that needs to be taken care of
        /// </summary>
        /// <param name="jobStateBase"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private SourceChanges HandleErrorState(DatabaseJobState jobStateBase, SqlDatabaseConnectorJobConfiguration configuration)
        {
            if (jobStateBase.ErrorCount > ErrorCountLimit)
            {
                var pausingMessage = $"Job has been paused due to the Error count limit's been reached: {ErrorCountLimit}. The error count has now been reset to 0.";
                jobStateBase.State = JobState.JobState.Paused;
                jobStateBase.Message = pausingMessage;
                jobStateBase.ErrorCount = 0;
                _stateService.SaveState(jobStateBase);
                Log.Logger.Warning(pausingMessage);
                return new SourceChanges();
            }
            var lastKnownState = jobStateBase.LastWorkingState;
            if (lastKnownState == JobState.JobState.Discovering)
                lastKnownState = JobState.JobState.IncrementalCrawling;
            Log.Logger.Warning($"{jobStateBase.Name}: State is in ERROR mode. Retrying (retry number {jobStateBase.ErrorCount}) by setting it to {lastKnownState}");
            jobStateBase.State = lastKnownState;
            jobStateBase.ErrorCount++;
            _stateService.SaveState(jobStateBase);
            return HandleState(jobStateBase,configuration);
        }

        private SourceChanges FetchDocumentsUsingIncremental(SqlDatabaseConnectorJobConfiguration config, DatabaseJobState jobStateBase)
        {
            //Not 100% threadsafe. But its good enough. If two concurrent threads do a discover, it'll still work fine
            var stateRightNow = new DatabaseJobState(_stateService.LoadState(config.JobName));
            if (stateRightNow.State == JobState.JobState.Discovering)
            {
                Log.Logger.Information(
                    $"{config.JobName}: State in DISCOVER phase. Another job is already at it. Doing nothing.");
                return new SourceChanges( );
            }
            jobStateBase.Status = JobStatus.Ok;
            jobStateBase.State = JobState.JobState.Discovering;
            _stateService.SaveState(jobStateBase);
            Log.Logger.Information($"{config.JobName}: Fetching incremental documents");

            var idsToFetch = FetchChangedIdsRecursively(config, EventType.Add, false, jobStateBase).ToList();

            IList<AddDocument> adds = null;
            if (idsToFetch.Any()) {
                var numBatches = _workTaskService.WriteTasks(config.JobName, config.BatchSize, idsToFetch);
                jobStateBase.BatchCount = numBatches;
                jobStateBase.LastDiscoverDate = DateTime.UtcNow;
                jobStateBase.LastDiscoverBatchCount = numBatches;

                if (idsToFetch.Count <= config.BatchSize)
                {
                    var task = _workTaskService.GetNextTask(config.JobName);
                    jobStateBase.BatchCount--;
                    adds = FetchDocumentsUsingWorkTask(config, task).Adds;
                }
            }

            var plainState = _stateService.LoadState(config.JobName);
            jobStateBase = new DatabaseJobState(plainState);
            var idFieldsToDelete = FetchChangedIdsRecursively(config, EventType.Delete, true, jobStateBase)
                            .Select(x=> new Field(config.MainTable.PrimaryKeyName, x)).ToList();
            

            jobStateBase.State = JobState.JobState.IncrementalCrawling;
            _stateService.SaveState(jobStateBase);
            
            return new SourceChanges(
                adds ?? new List<AddDocument>(),
                idFieldsToDelete);
        }

        private List<string> FetchChangedIdsRecursively(SqlDatabaseConnectorJobConfiguration config, EventType eventType, bool saveState, DatabaseJobState stateBase)
        {
            var changedIds = GetIntegration(config).DiscoverIncrementalIds(config, ref stateBase, eventType).ToList();
            if(saveState)
                _stateService.SaveState(stateBase);

            foreach (var foreignSource in config.ForeignSources)
            {
                var ids = FetchChangedIdsRecursively(foreignSource.SqlDatabaseConnectorConfiguration, eventType, saveState, stateBase);
                if(!ids.Any())
                    continue;
                var mainTableIds = GetIntegration(config).DiscoverMainTableIds(config, foreignSource.Relation.Key, ids.ToList());
                changedIds.AddRange(mainTableIds);
            }
            return changedIds.Distinct().ToList();
        }
         
        private SourceChanges FetchDocumentsUsingWorkTask(
            SqlDatabaseConnectorJobConfiguration config, WorkTask workTask)
        {
            Log.Logger.Information($"{config.JobName}: Fetching documents using ids in worktask: {workTask.Id}");
            try
            {
                //Get the documents from the data view
                var documents = FetchDocumentsRecursivly(
                    config,
                    config.MainTable.PrimaryKeyName,
                    config.MainTable.PrimaryKeyIsInteger,
                    workTask.Instructions);

                //marks the work task as completed (deletes the task)
                _workTaskService.TaskCompleted(workTask);

                //_stateService.SaveState(jobState, true);
                return new SourceChanges(documents, new List<Field>());
            }
            catch(Exception e)
            {
                Log.Logger.Warning($"Reverting work task to its original state due to error ({e.Message}");
                _workTaskService.RevertTask(workTask);
                var state = GetCurrentJobState(config);
                state.BatchCount++;
                _stateService.SaveState(state);
                throw;
            }

        }

        private IList<AddDocument> FetchDocumentsRecursivly(SqlDatabaseConnectorJobConfiguration config, string constraintColumn, bool constraintIsIntType, IList<string> ids)
        {
            var documents = GetIntegration(config).FetchDocuments(config, constraintColumn, constraintIsIntType, ids).ToList();
            foreach (var foreignSource in config.ForeignSources)
            {
                var foreignRowsToFetch = documents.SelectMany(x => x.Fields)
                                                  .Where(x => x.Name.Equals(foreignSource.Relation.Key))
                                                  .Select(x=>x.Value).ToList();
                if(foreignRowsToFetch.Count == 0)
                    continue;
                var foreignDocuments = FetchDocumentsRecursivly(
                    foreignSource.SqlDatabaseConnectorConfiguration, foreignSource.Relation.Value,foreignSource.RelationIsInteger, foreignRowsToFetch);
                documents.MergeWithDocuments(foreignDocuments, foreignSource.Relation, foreignSource.SqlDatabaseConnectorConfiguration.JobName);
            }
            return documents;
        }

        private void SetUpJobStateRecursively(SqlDatabaseConnectorJobConfiguration config, JobState.JobState jobState, string description = "")
        {
            var state = new DatabaseJobState
            {
                Name = config.JobName,
                State = jobState,
                LastExecutionDate = DateTime.UtcNow
            };
            if(!string.IsNullOrEmpty(description))
                state.Fields.Add(new Field("Description", description));

                foreach (var changeTable in config.ChangeTables)
                    state.SetChangetableVersionThreshold(changeTable.TableName,
                        GetIntegration(config).GetLastChangeVersion(config, changeTable));
                foreach (var eventTable in config.EventTables)
                    state.SetChangetableVersionThreshold(eventTable.TableName,
                        GetIntegration(config).GetLastChangeVersion(config, eventTable));

                _stateService.SaveState(state);

                foreach (var relatingSource in config.ForeignSources)
                {
                    SetUpJobStateRecursively(relatingSource.SqlDatabaseConnectorConfiguration,
                        JobState.JobState.ForeignTable,
                        $"{(string.IsNullOrEmpty(description) ? "" : description + " -> ")} Relating to {config.MainTable.TableName}.{relatingSource.Relation.Key} with {relatingSource.Relation.Value}");
                }
          
        }

        private IDatabaseIntegration GetIntegration(SqlDatabaseConnectorJobConfiguration config)
        {
            if(!_databaseIntegrations.ContainsKey(config.IntegrationType))
                throw new Exception(string.Format("No integration logic for {0} has been installed. " +
                                                  "Set it up in your bootstrapper. " +
                                                  "You currently have the following installed: {1}"
                    , config.IntegrationType, string.Join(", ",_databaseIntegrations.Keys)));
            return _databaseIntegrations[config.IntegrationType];
        }

        /// <summary>
        /// Tries to get the current job state
        /// If no state exists, a new job state will be created and initiated
        /// </summary>
        /// <param name="config"></param>
        private DatabaseJobState GetCurrentJobState(SqlDatabaseConnectorJobConfiguration config)
        {
            var state = _stateService.LoadState(config.JobName);

            if (state != null)
                return new DatabaseJobState(state);

            SetUpJobStateRecursively(config, JobState.JobState.InitialCrawling);
            state = _stateService.LoadState(config.JobName);
            if (state != null)
                return new DatabaseJobState(state);

            throw new Exception(
                $"Unable to find the job status file for {config.JobName} even though it was just created. ");
        }

        private void SetErrorState(SqlDatabaseConnectorJobConfiguration config, Exception e)
        {
            try
            {
                var jobState = GetCurrentJobState(config);
                jobState.SetErrorState(e);
                if (jobState.State != JobState.JobState.Error)
                    jobState.LastWorkingState = jobState.State;
                jobState.State = JobState.JobState.Error;
                _stateService.SaveState(jobState);
            }
            catch (Exception newExceptione)
            {
                Log.Error(newExceptione, "Error when saving error state: {message} for job {jobName}", newExceptione.Message, config.JobName);
                var errorState = new DatabaseJobState
                {
                    Name = config.JobName,
                    State = JobState.JobState.Paused,
                    LastExecutionDate = DateTime.UtcNow,
                    ErrorCount = 1,
                    LastWorkingState = JobState.JobState.Paused,
                    RecentErrorDate = DateTime.UtcNow,
                    Message = $"Job threw an exception, {e.Message}, and then threw an exception when trying to save the error state: {newExceptione.Message}"
                };
                _stateService.SaveState(errorState);
            }
        }
    }
}
