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

namespace Bifrost.Common.ApplicationServices.Sql
{
    public class SqlStateService : IStateService
    {
        private readonly DatabaseBase _database;

        public SqlStateService(StateDatabase database)
        {
            _database = database;
            database.SetupOnce();
        }


        public void SaveState(State state)
        {
            using (var connection = _database.GetOpenConnection())
            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                var stateModels = state.ToModels();
                var existingStateModels = connection.Query<StateEntryModel>(
                    string.Format("SELECT * FROM {0} WHERE StateName =@stateName",
                        StateEntryTable.TableName),
                    new {stateName = state.Name},
                    transaction).ToList();

                var newStates = stateModels.Where(s => existingStateModels.All(x => x.ParameterName != s.ParameterName));
                if (newStates.Any())
                    connection.Insert(newStates, transaction);

                
                if (existingStateModels.Any())
                {
                    foreach (var existingState in existingStateModels)
                    {
                        var newState = stateModels.FirstOrDefault(x =>
                            x.ParameterName == existingState.ParameterName
                            && x.ParameterValue != existingState.ParameterValue);
                        if (newState != null)
                        {
                            existingState.ParameterValue = newState.ParameterValue;
                            connection.Update(existingState, transaction);
                        }
                    }
                }
                transaction.Commit();
            }
        }

        public void DeleteState(string id)
        {
            using (var connection = _database.GetOpenConnection())
            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                connection.Execute(
                    string.Format("DELETE FROM {0} WHERE StateName=@stateName", StateEntryTable.TableName),
                    new {stateName = id.ToLower()},
                    transaction);
                transaction.Commit();
            }
        }

        public State LoadState(string id)
        {
            using (var connection = _database.GetOpenConnection())
            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                var stateEntries = connection.Query<StateEntryModel>(
                    string.Format("SELECT * FROM {0} WHERE StateName=@stateName", StateEntryTable.TableName),
                    new {stateName = id},
                    transaction).ToList();

                transaction.Commit();
                if (!stateEntries.Any())
                    return null;
                return stateEntries.ToDomain(id);
            }
        }

        public IList<State> LoadStates()
        {
            using (var connection = _database.GetOpenConnection())
            {
                var stateEntries = connection.Query<StateEntryModel>(
                    string.Format("SELECT * FROM {0}", StateEntryTable.TableName));
                return stateEntries.ToDomain();
            }
        }
    }
}