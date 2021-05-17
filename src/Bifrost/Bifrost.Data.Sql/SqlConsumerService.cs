using System.Collections.Generic;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Domain;
using Bifrost.Data.Sql.Databases;
using Bifrost.Data.Sql.Databases.Bifrost.Mappers;
using Bifrost.Data.Sql.Databases.Bifrost.Models;
using Bifrost.Data.Sql.Databases.Bifrost.Objects.Tables;

namespace Bifrost.Data.Sql
{
    public class SqlConsumerService : IConsumerService
    {
         private readonly AsgardDatabase _AsgardDatabase;


        public SqlConsumerService(AsgardDatabase AsgardDatabase)
        {
            _AsgardDatabase = AsgardDatabase;
            AsgardDatabase.SetupOnce();
        }


        public Consumer GetConsumerByName(string name)
        {
            using (var connection = _AsgardDatabase.GetOpenConnection())
            {
                var sql = string.Format("SELECT TOP(1) * FROM {0} " +
                                        "WHERE Name=@name",
                                        ConsumerTable.TableName);
                var model = connection.Query<ConsumerModel>(sql, new { name }).FirstOrDefault();
                if (model == null || model.Name != name) //mitigate different collations
                    return null;
                return model.ToDomain();
            }
        }


        public Consumer GetConsumerByApiKey(string apiKey)
        {
            using (var connection = _AsgardDatabase.GetOpenConnection())
            {
                var sql = string.Format("SELECT TOP(1) * FROM {0} " +
                                        "WHERE ApiKey=@apiKey",
                                        ConsumerTable.TableName);
                var model = connection.Query<ConsumerModel>(sql, new { apiKey }).FirstOrDefault();
                if (model == null || model.ApiKey != apiKey) //mitigate different collations
                    return null;
                return model.ToDomain();
            }
        }

        public Consumer GetConsumerByJobName(string jobName)
        {
            using (var connection = _AsgardDatabase.GetOpenConnection())
            {
                var sql = string.Format("SELECT TOP(1) * FROM {0} " +
                                        "WHERE JobsCsv LIKE @jobName " +
                                        "ORDER BY IsAdmin asc",
                                        ConsumerTable.TableName);
                var model = connection.Query<ConsumerModel>(sql, new { jobName=string.Format("%{0}%",jobName) }).FirstOrDefault();
                if (model == null) 
                    return null;
                return model.ToDomain();
            }
        }

        public IList<Consumer> GetAllConsumers()
        {
            using (var connection = _AsgardDatabase.GetOpenConnection())
            {
                var sql = string.Format("SELECT * FROM {0} ",
                                        ConsumerTable.TableName);
                var models = connection.Query<ConsumerModel>(sql);
                if (models == null)
                    return new List<Consumer>();
                return models.Select(x => x.ToDomain()).ToList();
            }
        }

        public void SaveOrUpdate(Consumer consumer)
        {
            using (var connection = _AsgardDatabase.GetOpenConnection())
            using (var transaction = connection.BeginTransaction())
            {
                var sql = string.Format("SELECT TOP(1) * FROM {0} WHERE  Name =@name", ConsumerTable.TableName);
                var existingConsumer= connection.Query<ConsumerModel>(sql, new {apiKey=consumer.ApiKey, name =consumer.Name}, transaction).FirstOrDefault();

                
                if (existingConsumer == null)
                {
                    connection.Insert(consumer.ToModel(), transaction);
                }
                else
                {
                    connection.Update(consumer.ToModel(), transaction);
                }
                transaction.Commit();
            }
        }

        public void DeleteConsumer(string name)
        {
             using (var connection = _AsgardDatabase.GetOpenConnection())
             using (var transaction = connection.BeginTransaction())
             {
                 var sql = string.Format("DELETE FROM {0} WHERE Name=@name", ConsumerTable.TableName);
                 connection.Execute(sql, new {name}, transaction);
                 transaction.Commit();
             }
        }
    }
}
