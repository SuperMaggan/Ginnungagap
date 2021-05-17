using System.Collections.Generic;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Bifrost.Common.ApplicationServices.Sql.Common;
using Bifrost.Common.ApplicationServices.Sql.Databases;
using Bifrost.Common.ApplicationServices.Sql.Models;
using Bifrost.Common.ApplicationServices.Sql.Models.Mappers;
using Bifrost.Common.ApplicationServices.Sql.Tables;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.ApplicationServices.Helpers.Base;
using Bifrost.Common.Core.Domain;
using Serilog;

namespace Bifrost.Common.ApplicationServices.Sql
{
    public class SqlProcessInformationService : ProcessInformationServiceBase, IProcessInformationService
    {
        private readonly DatabaseBase _database;

        public SqlProcessInformationService(IProcessInformationCollector[] collectors,
            ProcessInformationDatabase database)
            : base(collectors, 15)
        {
            _database = database;
            _database.SetupOnce();
        }


       

        public ProcessInformation Get(string id)
        {
            using (var connection = _database.GetOpenConnection())
            {
                var sql = $"SELECT * FROM {ProcessInformationTable.TableName} Where Id=@id ";
                var model = connection.Query<ProcessInformationModel>(sql, new {id}).FirstOrDefault();
                return model?.ToDomain();
            }
        }

        /// <summary>
        ///     Retrieves all persisted process informations
        /// </summary>
        /// <returns></returns>
        public IList<ProcessInformation> Get()
        {
            using (var connection = _database.GetOpenConnection())
            {
                var sql = string.Format("SELECT * FROM {0} ",
                    ProcessInformationTable.TableName);
                var models = connection.Query<ProcessInformationModel>(sql);
                if (models == null)
                    return new List<ProcessInformation>();
                return models.Select(x => x.ToDomain()).ToList();
            }
        }

        /// <summary>
        ///     Presists the current process information
        ///     This is the same as Save( GetCurrent() )
        /// </summary>
        public void SaveCurrent(string processType)
        {
            Save(CreateSnapshot(processType));
        }

        /// <summary>
        ///     Presists the given processInformation
        /// </summary>
        /// <param name="processInformation"></param>
        public void Save(ProcessInformation processInformation)
        {
            using (var connection = _database.GetOpenConnection())
            {
                var sql = $"SELECT TOP(1) * FROM {ProcessInformationTable.TableName} WHERE  Id =@id";
                var existingInfo = connection.Query<ProcessInformationModel>(sql, new {id = processInformation.Id}).FirstOrDefault();

                var model = processInformation.ToModel();
                if (existingInfo == null)
                {
                    connection.Insert(model);
                }
                else
                {
                    connection.Update(model);
                }
            }
        }

        /// <summary>
        ///     Delete all persisted Process information
        /// </summary>
        public void Delete()
        {
            using (var connection = _database.GetOpenConnection())
            using (var transaction = connection.BeginTransaction())
            {
                var sql = string.Format("DELETE FROM {0}", ProcessInformationTable.TableName);
                connection.Execute(sql, transaction);
                transaction.Commit();
            }
        }

        /// <summary>
        ///     Delete the information with the given Id
        /// </summary>
        /// <param name="id"></param>
        public void Delete(string id)
        {
            using (var connection = _database.GetOpenConnection())
            using (var transaction = connection.BeginTransaction())
            {
                var sql = string.Format("DELETE FROM {0} WHERE Id=@id", ProcessInformationTable.TableName);
                connection.Execute(sql, new {id}, transaction);
                transaction.Commit();
            }
        }
    }
}