using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bifrost.Core.Domain.Document;
using Bifrost.Common.Core.ApplicationServices;
using Serilog;
using Serilog.Core;

namespace Bifrost.Processing.CsScript
{
    public class CsScriptProcessingService
    {
        private const string ScriptSuffix = "Script.cs";
        private readonly string _defaultScriptName = "DefaultScript";
        private readonly string _scriptDirectory;
        private readonly ITextExtractionService _textExtractorService;

        public CsScriptProcessingService(ITextExtractionService textExtractorService)
        {
            _textExtractorService = textExtractorService;
            var path = System.Reflection.Assembly.GetEntryAssembly().Location;
            _scriptDirectory = Path.Combine(Path.GetDirectoryName(path), "Scripts");
        }



        public IEnumerable<DocumentBase> ProcessDocuments(IEnumerable<DocumentBase> documentBatch, string domain)
        {
            Log.Debug($"Starting CsScript processing a batch of {domain} documents");
            var script = GetScript(domain);
            
            foreach (var document in documentBatch)
            {
                if (script == null)
                    yield return document;
                else { 
                    DocumentBase processedDocument = null;
                    try
                    {
                        if (document is AddDocument)
                            processedDocument = script.ProcessDocument(document.Clone() as AddDocument,
                                _textExtractorService);
                        else
                            processedDocument = script.ProcessDelete(document.Clone() as DeleteDocument);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(string.Format("Error when processing document {0} from domain {1} ({2}). {3}",
                            document.Id,
                            domain,
                            domain + ScriptSuffix,
                            string.Format("{0}\n{1}", e.Message, e.StackTrace)));
                    }
                    yield return processedDocument;
                }
            }
            
            
        }

        private IScript GetScript(string domain)
        {
            var scope = GetScriptScope(domain);
            if (string.IsNullOrEmpty(scope))
                return null;
           
            string code = File.ReadAllText(scope);
            var script = (IScript)CSScript.LoadCode(code).CreateObject("*");
            return script;
        }

        private string GetScriptScope(string domain)
        {
            if (!Directory.Exists(_scriptDirectory))
                return "";
            var fileName = domain + ScriptSuffix;
            
            var scriptPath = Directory.GetFiles(_scriptDirectory, fileName, SearchOption.AllDirectories).FirstOrDefault();


            if (string.IsNullOrEmpty(scriptPath))
            {

                var defaultScriptPath = Directory.GetFiles(_scriptDirectory, _defaultScriptName+".cs", SearchOption.AllDirectories).FirstOrDefault();
                if (File.Exists(defaultScriptPath))
                    return defaultScriptPath;
                return null;

            }
            return scriptPath;
        }

        public override string ToString()
        {
            return string.Format("{0}: Script folder {1} ",
                this.GetType().Name,
                _scriptDirectory);
        }

    }
}
