using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bifrost.Core.Domain.Document;
using Bifrost.Core.Pipeline.Interfaces;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain;
using Bifrost.Common.Core.Domain.TextExtraction;
using Serilog;

namespace Bifrost.Core.Pipeline.DocumentSteps
{
    public class BinaryDataExtractionPipelineStep : IDocumentPipelineStep
    {
        private readonly ITextExtractionService _textExtractorService;

        public BinaryDataExtractionPipelineStep(ITextExtractionService textExtractorService)
        {
            _textExtractorService = textExtractorService;
        }

        public IDocument Invoke(IDocument document, string domain)
        {
            if (document is AddDocument)
                HandleAddDocument((AddDocument)document);
            return document;

        }

        private void HandleAddDocument(AddDocument document)
        {
            var binaryFields = document.Fields.Where(x => x is BinaryField)
               .Cast<BinaryField>()
               .ToArray();

            foreach (var binaryField in binaryFields)
            {
                var extractedFields = ExtractFields(document.Id, binaryField, document.Domain);
                binaryField.Data = new byte[0];
                binaryField.Value = "";
                foreach (var extractedField in extractedFields)
                {
                    document.Fields.Add(extractedField);
                }
            }
        }

        private IEnumerable<Field> ExtractFields(string documentId, BinaryField binaryField, string domain)
        {
            try
            {
                using (var stream = new MemoryStream(binaryField.Data))
                {
                    var extractedDoc = _textExtractorService.ExtractText(
                        new BinaryDocumentStream(documentId,domain, stream));
                    if (extractedDoc.TextExtractionException != null)
                    {
                        Log.Warning(
                            $"Error when extracting binary field {binaryField.Name} from {documentId}: {extractedDoc.TextExtractionException}");
                        return new Field[]
                        {
                            new Field("ExtractionError",
                                $"Error when extracting binary field {binaryField.Name} from {documentId}: {extractedDoc.TextExtractionException}")
                        };
                    }
                    var doc = extractedDoc.ResultingDocument as AddDocument;


                    foreach (var field in doc.Fields)
                    {
                        field.Name = string.Format("{0}_extracted_{1}",
                            binaryField.Name,
                            field.Name.Replace(":", "-").Replace(" ", "-").Replace("(", "").Replace(")", ""));
                    }
                    return doc.Fields;
                }
            }
            catch (Exception e)
            {
                return new[]
                {
                    new Field("Error",
                        string.Format("Error when extracting text from binary field {0} in document {3} :{1} ({2})",
                        binaryField.Name,
                        e.Message,
                        e.InnerException != null ? e.InnerException.Message : "",
                        documentId))
                };
            }
        }

        public bool Enabled { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: Enabled ({1}) ",
                this.GetType().Name,
                Enabled);
        }
    }
}