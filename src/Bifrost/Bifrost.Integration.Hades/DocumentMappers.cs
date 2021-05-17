using System;
using System.Linq;
using Asgard.Common.Core.Domain;
using Asgard.Hades.Dto.Dto;

namespace Asgard.Integration.Hades
{
    public static class DocumentMappers
    {
        public static DocumentDto ToDto(this AddDocument document)
        {
            if (document == null)
                return null;
            if (string.IsNullOrEmpty(document.Id))
                throw new MissingFieldException("A Document with no Id was passed!");
            if (string.IsNullOrEmpty(document.Domain))
                throw new MissingFieldException("A Document with no Domain was passed!");

            return new DocumentDto
            {
                Id = document.Id,
                Domain = document.Domain,
                Fields = document.Fields.Select(ToDto).ToList()
            };
        }

        public static FieldDto ToDto(this Field field)
        {
            if (string.IsNullOrEmpty(field.Name))
                throw new MissingFieldException("A Field with no Name was passed!");
            return new FieldDto(field.Name, field.Value);
        }
        public static DeleteDocumentDto ToDto(this DeleteDocument deleteDocument)
        {
            if (deleteDocument == null)
                return null;
            if (string.IsNullOrEmpty(deleteDocument.Id))
                throw new ArgumentException("A Document with no Id was passed!");
            if (string.IsNullOrEmpty(deleteDocument.Domain))
                throw new ArgumentException("A Document with no Domain was passed!");

            return new DeleteDocumentDto(deleteDocument.Id, deleteDocument.Domain);
        }
    }
}