using System;
using System.Collections.Generic;
using System.Linq;
using Bifrost.Core.Domain.Document;
using Bifrost.Core.Pipeline.Interfaces;
using Bifrost.Common.Core.Domain;
using Serilog;

namespace Bifrost.Core.Pipeline.DocumentSteps
{
    /// <summary>
    /// Handles so called "soft deletes", that is when the source system isn't really deleting the data but
    /// instead sets a flag, eg. "IsDeleted = true"
    /// This step will change a soft deleted AddDocument to a DeleteDocument
    /// </summary>
    public class SoftDeletePipelineStep : IDocumentPipelineStep
    {
      

        private readonly List<string> _hardCodedSoftDeleteNames = new List<string>(){
            "IsHidden",
            "IsDeleted",
            "Deleted"
            };
        private readonly List<string> _hardCodedSoftDeleteValues = new List<string>(){
            "true",
            "1"
            }; 

        public IDocument Invoke(IDocument document, string domain)
        {
            if (document is AddDocument)
                document = HandleAddDocument((AddDocument) document);
            return document;
        }

        private IDocument HandleAddDocument(AddDocument document)
        {
            var softDeleteField = document.Fields.FirstOrDefault(f =>
                _hardCodedSoftDeleteNames.Any(n => n.Equals(f.Name, StringComparison.CurrentCultureIgnoreCase))
                );
            if (softDeleteField == null)
                return document;
            if (string.IsNullOrEmpty(softDeleteField.Value))
                return document;
            if (!_hardCodedSoftDeleteValues.Any(
                val => val.Equals(softDeleteField.Value, StringComparison.CurrentCultureIgnoreCase))) 
                return document;

            Log.Debug(
                $"SoftDeletePipelineStep identified a document ({document.Id}) as soft deleted due to field '{softDeleteField.Name}'='{softDeleteField.Value}'");
            return new DeleteDocument("IDontKnowMyDeleteFieldName_FixMePlz_WorksForHadesThough", document.Id);
        }


        public bool Enabled { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: Enabled ({1}). '{2}' must be either '{3}' ",
                this.GetType().Name,
                Enabled,
                string.Join("', '",_hardCodedSoftDeleteNames),
                string.Join("', '", _hardCodedSoftDeleteValues));
        }
    }
}