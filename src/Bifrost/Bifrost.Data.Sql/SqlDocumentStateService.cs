using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Domain.Document;
using Bifrost.Data.Sql.Databases;
using Bifrost.Data.Sql.Databases.Bifrost.Models;
using Bifrost.Data.Sql.Databases.Bifrost.Objects.StoredProcedures;
using Bifrost.Data.Sql.Databases.Bifrost.Objects.Tables;
using Bifrost.Common.ApplicationServices.Sql.Common;
using Serilog;

namespace Bifrost.Data.Sql
{
    public class SqlDocumentStateService : IDocumentStateService
    {
         private readonly AsgardDatabase _AsgardDatabase;
        private readonly IMapper<DocumentState, DocumentStateModel> _documentStateMapper;


        public SqlDocumentStateService(AsgardDatabase AsgardDatabase,
           IMapper<DocumentState, DocumentStateModel> documentStateMapper)
        {
            _AsgardDatabase = AsgardDatabase;
            _documentStateMapper = documentStateMapper;
            AsgardDatabase.SetupOnce();
        }



        public DocumentState Get(string category, string documentId)
        {
            using (var connection = _AsgardDatabase.GetOpenConnection())
            {
                var sql =
                    $"SELECT TOP 1 * FROM {DocumentStateTable.TableName} " +
                    $"WHERE Category=@category AND DocumentId=@documentId";
                var state = connection.Query<DocumentStateModel>(sql, new {category,documentId}).FirstOrDefault();
                return _documentStateMapper.MapBack(state);
            }
        }

        /// <summary>
        /// Pushes all documents that should be verified to the Queue
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns> 
        public int PushVerifyDocumentsToQueue(string category)
        {
            using (var connection = _AsgardDatabase.GetOpenConnection())
            {
                return PushVerifyDocumentToQueueProcedure.Execute(connection,category);
            }
        }


        public void SaveOrUpdate(string category, DocumentState documentState)
        {
            using (var connection = _AsgardDatabase.GetOpenConnection())
            {
                var existSql = 
                    $"SELECT * FROM {DocumentStateTable.TableName} " +
                    $"WHERE Category=@category AND DocumentId=@documentId";
                var exist = connection.Query<DocumentStateModel>(existSql, new {category, documentState.DocumentId}).FirstOrDefault();

                var model = _documentStateMapper.Map(documentState);
                model.Category = category;
                if (exist != null)
                {
                    connection.Update(model);
                }
                else
                {
                    connection.Insert(model);
                }
            }
        }

        public void Delete(string category, string documentId)
        {
            using (var connection = _AsgardDatabase.GetOpenConnection())
            using (var transaction = connection.BeginTransaction())
            {
                var deleted =
                    connection.Execute(
                        $"DELETE FROM {DocumentStateTable.TableName} " +
                        $"WHERE Category=@category AND DocumentId=@documentId",
                        new {category, documentId},
                        transaction);
                transaction.Commit();
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
                var deleted = connection.Execute($"DELETE FROM {DocumentStateTable.TableName} " +
                                                 $"WHERE Category=@category",
                    new { category },
                    transaction);
                transaction.Commit();
            }
        }
    }
}
