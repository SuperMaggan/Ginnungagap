using Bifrost.Core.Domain;
using Bifrost.Data.Sql.Databases.Bifrost.Models;
using Bifrost.Common.ApplicationServices.Sql.Common;

namespace Bifrost.Data.Sql.Databases.Bifrost.Mappers
{
    public class QueueItemMapper: IMapper<QueueItem, QueueItemModel>
    {



        public QueueItemModel Map(QueueItem sourceObject)
        {
            return new QueueItemModel()
                       {
                           CreateDate = sourceObject.CreateDate,
                           DocumentId = sourceObject.DocumentId,
                           OptionalData = sourceObject.OptionalData
                       };
        }

        public QueueItem MapBack(QueueItemModel sourceObject)
        {
            return new QueueItem()
                       {
                           CreateDate = sourceObject.CreateDate,
                           DocumentId = sourceObject.DocumentId,
                           OptionalData = sourceObject.OptionalData
                       };
        }
    }
}