using System.Collections.Generic;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Domain;
using Bifrost.Data.Sql.Databases;
using Bifrost.Data.Sql.Databases.Bifrost.Models;
using Bifrost.Data.Sql.Databases.Bifrost.Objects.Tables;
using Bifrost.Common.ApplicationServices.Sql.Common;
using Bifrost.Common.ApplicationServices.Sql.Common.Extensions;

namespace Bifrost.Data.Sql
{
    public class SqlQueueService : IQueueService
    {
         private readonly AsgardDatabase _AsgardDatabase;
        private readonly IMapper<QueueItem, QueueItemModel> _queueMapper;


        public SqlQueueService(AsgardDatabase AsgardDatabase,
            IMapper<QueueItem, QueueItemModel> queueMapper)
        {
            _AsgardDatabase = AsgardDatabase;
            _queueMapper = queueMapper;
            _AsgardDatabase.SetupOnce();
        }


        public int Push(string category, IList<QueueItem> items)
        {
            using (var connection = _AsgardDatabase.GetOpenConnection())
            {
                var query =
                    $"SELECT DocumentId FROM {QueueItemTable.TableName}" +
                    $" WHERE Category = @category " +
                    $"AND DocumentId in {items.Select(x => x.DocumentId).ToSqlInConstraint()}";
                var existingItems = connection.Query<string>(
                    query,
                    new {category}).ToList();
                var itemModels = items.Where(x => !existingItems.Contains(x.DocumentId)).Select(x =>
                { 
                    var model = _queueMapper.Map(x);
                    model.Category = category;
                    return model;
                });
               
                if(existingItems.Count < items.Count)
                    connection.Insert(itemModels);
                return items.Count - existingItems.Count;
            }
        }

        public IList<QueueItem> Pop(string category, int numberToPop)
        {
            using (var connection = _AsgardDatabase.GetOpenConnection())
            using(var transaction = connection.BeginTransaction())
            {
                var sql = $"SELECT TOP ({numberToPop}) * FROM {QueueItemTable.TableName} WHERE Category=@category";

                var itemModels = connection.Query<QueueItemModel>(sql,
                    new {category},
                    transaction
                ).ToList();

                if (itemModels.Count == 0)
                    return new List<QueueItem>();
                var deleteSql = string.Format("DELETE FROM {0} WHERE Id in {1}", 
                                                QueueItemTable.TableName,
                                              itemModels.Select(x => x.Id).ToSqlInConstraint());

                connection.Execute(deleteSql, null, transaction);
                transaction.Commit();
                return itemModels.Select(_queueMapper.MapBack).ToList();
            }

        }

        public int Count(string category)
        {
            using (var connection = _AsgardDatabase.GetOpenConnection())
            {
                return connection.Query<int>(
                    $"SELECT COUNT(DocumentId) FROM {QueueItemTable.TableName} WHERE Category=@category",
                    new {category}).First();
            }
        }

        /// <summary>
        /// Delete all items related to the given category
        /// </summary>
        /// <param name="category"></param>
        public void Delete(string category)
        {
            using (var connection = _AsgardDatabase.GetOpenConnection())
            using (var transaction = connection.BeginTransaction())
            {
                var deleted = connection.Execute($"DELETE FROM {QueueItemTable.TableName} WHERE Category=@category",
                    new {category},
                    transaction);
                transaction.Commit();
            }
        }
    }
}
