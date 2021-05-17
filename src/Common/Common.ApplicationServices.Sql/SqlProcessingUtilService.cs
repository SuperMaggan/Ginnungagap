using System;
using System.Data;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Bifrost.Common.ApplicationServices.Sql.Databases;
using Bifrost.Common.ApplicationServices.Sql.Models;
using Bifrost.Common.ApplicationServices.Sql.Tables;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain.DocumentProcessing;

namespace Bifrost.Common.ApplicationServices.Sql
{
    public class SqlProcessingUtilService :  IProcessingUtilService
    {
        private readonly ProcessingUtilDatabase _database;

        public SqlProcessingUtilService(ProcessingUtilDatabase database)
        {
            _database = database;
            _database.SetupOnce();
        }


        public void SaveUtilObject<T>(T utilObject)
            where T : IProcessingUtilObject
        {
            using (var conn = _database.GetOpenConnection())
            using (var transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                var key = utilObject.Key;
                var sql = string.Format("SELECT TOP 1 * FROM {0} WHERE [Key] = @key",
                    ProcessingUtilObjectsTable.TableName);
                var existingObject = conn.Query<ProcessingUtilObjectModel>(sql, new { key }, transaction).FirstOrDefault();
                var model = MapToModel(utilObject);
                if (existingObject == null)
                {
                    conn.Insert(model, transaction);
                }
                else
                {
                    conn.Update(model, transaction);
                }
                transaction.Commit();
            }

        }

        public T GetUtilObject<T>(string key) where T : IProcessingUtilObject, new()
        {
            using (var conn = _database.GetOpenConnection())
            {
                var sql = string.Format("SELECT TOP 1 * FROM {0} WHERE [Key] = @key", ProcessingUtilObjectsTable.TableName);
                var fetchedModel = conn.Query<ProcessingUtilObjectModel>(sql, new { key }).FirstOrDefault();
                if (fetchedModel == null)
                    return default(T);

                return MapFromModel<T>(fetchedModel);
            }
        }

        public void DeleteUtilObject(string key)
        {
            using (var conn = _database.GetOpenConnection())
            using (var transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                var model = new ProcessingUtilObjectModel()
                {
                    Key = key
                };
                conn.Delete(model, transaction);
                transaction.Commit();
            }

        }

        public virtual TCustomUtilService GetCustomUtilService<TCustomUtilService>()
            where TCustomUtilService : class
        {
            throw new NotSupportedException("When using a CustomUtilService you need to overload the GetCustomUtilService() method");
        }

        private ProcessingUtilObjectModel MapToModel<T>(T utilObject)
            where T : IProcessingUtilObject
        {
            return new ProcessingUtilObjectModel()
            {
                Key = utilObject.Key,
                ObjectType = utilObject.GetType().Name,
                Value = utilObject.Serialize()
            };
        }

        private T MapFromModel<T>(ProcessingUtilObjectModel model)
            where T : IProcessingUtilObject, new()
        {
            var utilObject = new T();
            utilObject.Deserialize(model.Key, model.Value);
            return utilObject;
        }
    }
}