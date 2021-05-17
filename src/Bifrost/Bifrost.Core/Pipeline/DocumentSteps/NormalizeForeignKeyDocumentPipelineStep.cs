using System;
using System.Collections.Generic;
using System.IO;
using Bifrost.Core.Domain.Document;
using Bifrost.Core.Pipeline.Interfaces;
using Bifrost.Common.Core.Domain;
using Serilog;

namespace Bifrost.Core.Pipeline.DocumentSteps
{
    public class NormalizeForeignKeyDocumentPipelineStep : IDocumentPipelineStep
    {

        private Dictionary<string, ForeignKeyField> _foreignKeys;
        private const string ForeignKeyName = "JOINKEY";
        private const string TargetKeyName = "JOINTARGETKEY";
       
        public bool Enabled { get; set; }

        public NormalizeForeignKeyDocumentPipelineStep()
        {
            Init();
        }

        private void Init()
        {
            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
            var uri = new UriBuilder(entryAssembly?.Location);
            var dirPath = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
            var filePath = Path.Combine(dirPath, "ForeignKeyMappings.txt");
            if (!File.Exists(filePath))
                Log.Error(
                    $"Cant find the file required for NormalizeForeignKeyDocumentPipelineStep at {filePath} .Step will not be used.");
            ParseMappings(filePath);

        }

        private void ParseMappings(string filePath)
        {
            _foreignKeys = new Dictionary<string, ForeignKeyField>();
            if (!File.Exists(filePath))
            {
                Enabled = false;
                return;
            }
            using (TextReader reader = File.OpenText(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                        continue;
                    var tokens = line.Split(new string[] { "->" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length != 2)
                        throw new FormatException("The ForeignKeyMappings.txt file needs to have mappings in the format 'keyField -> prefix_{0}' where {0} will be replaced with the field value");
                    
                    string targetField = string.Empty;
                    var targetTokens = tokens[0].Split(':');
                    if (targetTokens.Length == 2)
                    {
                        tokens[0] = targetTokens[1];
                        targetField = targetTokens[0];
                    }
                    _foreignKeys.Add(tokens[0].Trim(), new ForeignKeyField(tokens[0].Trim(), tokens[1].Trim(), targetField.Trim()));
                }
            }
            Enabled = true;
        }


        public IDocument Invoke(IDocument document, string domain)
        {
            if (Enabled && document is AddDocument)
                HandleAddDocument(document as AddDocument);
            return document;
        }

        private void HandleAddDocument(AddDocument document)
        {
            for (int index = 0; index < document.Fields.Count; index++)
            {
                var field = document.Fields[index];
                ForeignKeyField foreignKeyField;
                if (_foreignKeys.TryGetValue(field.Name, out foreignKeyField))
                {
                    document.Fields.Add(
                        new Field(ForeignKeyName, string.Format(foreignKeyField.MappingFormat, field.Value)));
                    if (!string.IsNullOrWhiteSpace(foreignKeyField.JoinTargetFieldName))
                        document.Fields.Add(
                            new Field(foreignKeyField.JoinTargetFieldName, string.Format(foreignKeyField.MappingFormat, field.Value)));
                }
            }
        }
        public override string ToString()
        {
            return string.Format("{0}: Enabled ({1}) ",
                this.GetType().Name,
                Enabled);
        }

        private class ForeignKeyField
        {
            /// <summary>
            /// Foreign key field name
            /// </summary>
            public string FieldName { get; set; }

            /// <summary>
            /// Format to map the value to
            /// </summary>
            public string MappingFormat { get; set; }

            /// <summary>
            /// Create an additional field with this name
            /// </summary>
            public string JoinTargetFieldName{ get; set; }

            public ForeignKeyField(string fieldName, string mappingFormat, string joinTargetFieldName)
            {
                FieldName = fieldName;
                MappingFormat = mappingFormat;
                JoinTargetFieldName = joinTargetFieldName;
            }
        }
    }
}
