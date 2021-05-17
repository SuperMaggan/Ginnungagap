using Bifrost.Core.Domain.Document;
using Bifrost.Data.Sql.Databases.Bifrost.Models;
using Bifrost.Common.ApplicationServices.Sql.Common;

namespace Bifrost.Data.Sql.Databases.Bifrost.Mappers
{
    public class DocumentStateMapper : IMapper<DocumentState, DocumentStateModel>
    {

        public DocumentStateModel Map(DocumentState sourceObject)
        {
            return new DocumentStateModel
            {
                           DocumentId = sourceObject.DocumentId,
                           HashValue = sourceObject.HashValue,
                           LastUpdated = sourceObject.LastUpdated,
                           LastVerified = sourceObject.LastVerified,
                           VerifyDate = sourceObject.VerifyDate,
                           OptionalData = sourceObject.OptionalData
                       };
        }

        public DocumentState MapBack(DocumentStateModel sourceObject)
        {
            if (sourceObject == null)
                return null;
            return new DocumentState
            {
                DocumentId = sourceObject.DocumentId,
                HashValue = sourceObject.HashValue,
                LastUpdated = sourceObject.LastUpdated,
                LastVerified = sourceObject.LastVerified,
                VerifyDate = sourceObject.VerifyDate,
                OptionalData = sourceObject.OptionalData
            };
        }
    }
}