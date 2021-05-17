using System;
using System.Collections.Generic;
using System.IO;
using Bifrost.Core.Domain.Document;
using Bifrost.Core.Pipeline.Interfaces;
using Bifrost.Common.Core.Domain;
using Serilog;

namespace Bifrost.Core.Pipeline.DocumentSteps
{
    public class FieldMapDocumentPipelineStep : IDocumentPipelineStep
    {
        
        private Dictionary<string, string> _fieldMoveMappings;
        private Dictionary<string, string> _fieldCopyMappings;

        public FieldMapDocumentPipelineStep()
        {
            Init();
        }

        private void Init()
        {
            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
            var uri = new UriBuilder(entryAssembly?.Location);
            var dirPath = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
            var filePath = Path.Combine(dirPath,"FieldMappings.txt");
            if (!File.Exists(filePath))
                Log.Error(
                    $"Cant find the file required for FieldMapDocumentPipelineStep at {filePath} .Step will not be used.");
            ParseMappings(filePath);

        }

        private void ParseMappings(string filePath)
        {
            _fieldMoveMappings = new Dictionary<string, string>();
            _fieldCopyMappings = new Dictionary<string, string>();
            if (!File.Exists(filePath))
            {
                Enabled = false;
                return;
            }
            using(TextReader reader = File.OpenText(filePath))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                        continue;
                    var typeTokens = line.Split(':');
                    if(typeTokens.Length != 2)
                        throw new Exception("A mapping must be at the format 'Move|Copy: fromField -> toField' and fromField must be uniquie");

                    var mapToken = typeTokens[1].Split(new string[] { "->" }, StringSplitOptions.RemoveEmptyEntries);
                    if (mapToken.Length != 2)
                        continue;
                    if(typeTokens[0].ToLower() == "move")
                    {
                        if (_fieldMoveMappings.ContainsKey(mapToken[0].Trim()))
                            throw new Exception("A mapping must be at the format 'Move|Copy: fromField -> toField' and fromField must be uniquie");
                        _fieldMoveMappings.Add(mapToken[0].Trim(), mapToken[1].Trim());
                    }
                    else if (typeTokens[0].ToLower() == "copy")
                    {
                        if (_fieldCopyMappings.ContainsKey(mapToken[0].Trim()))
                            throw new Exception("A mapping must be at the format 'Move|Copy: fromField -> toField' and fromField must be uniquie");
                        _fieldCopyMappings.Add(mapToken[0].Trim(), mapToken[1].Trim());
                    }
                }
            }
            Enabled = true;
        }


        public IDocument Invoke(IDocument document, string domain)
        {
            if (!Enabled)
                return document;
            if (document is DeleteDocument)
                return document;

            for (int index = 0; index < ((AddDocument) document).Fields.Count; index++)
            {
                var field = ((AddDocument)document).Fields[index];
                string newFieldName;
                if (_fieldCopyMappings.TryGetValue(field.Name, out newFieldName))
                {
                    ((AddDocument)document).Fields.Add(new Field(newFieldName, field.Value));
                }
                else if (_fieldMoveMappings.TryGetValue(field.Name, out newFieldName))
                {
                    field.Name = newFieldName;
                }
            }
            return document;
        }

        public bool Enabled { get; set; }
    }
}
