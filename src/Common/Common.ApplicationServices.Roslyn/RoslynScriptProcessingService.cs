using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using Bifrost.Common.ApplicationServices.Roslyn.Scripts;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.ApplicationServices.Implementations;
using Bifrost.Common.Core.Data;
using Bifrost.Common.Core.Domain;
using Bifrost.Common.Core.Domain.DocumentProcessing;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Serilog;
using Script = Bifrost.Common.Core.Domain.Script;

namespace Bifrost.Common.ApplicationServices.Roslyn
{
    public class RoslynScriptProcessingService : IScriptProcessingService
    {

        private readonly IScriptRepository _scriptRepository;
        private readonly IProcessingUtilService _utilService;
        private readonly ScriptProcessingSettings _settings;
        private readonly ConcurrentDictionary<string, ScriptWrapper> _compiledScriptCache;

        private readonly ICache<string, Script> _scriptCache;


        public RoslynScriptProcessingService(IScriptRepository scriptRepository, IProcessingUtilService utilService, ScriptProcessingSettings settings)
        {
            _scriptRepository = scriptRepository;
            _utilService = utilService;
            _settings = settings;

            //Cache scripts for one minute
            _compiledScriptCache = new ConcurrentDictionary<string, ScriptWrapper>();
            
            _scriptCache = new TimedMemoryCache<string, Script>(TimeSpan.FromSeconds(60));
        }

        public IDocument Process(IDocument document)
        {
            var script = GetScript(GetScriptName(document.Domain));
            var compiledScript = GetCompiledScript(script);
            return Process(document, compiledScript);
        }

        public IDocument Process(IDocument document, Script script)
        {
            var compiledScript = InterpretScript(script);
            return Process(document, compiledScript);
        }



        public IEnumerable<IDocument> Process(IList<IDocument> documents)
        {
            if (!documents.Any())
                yield break;
            var scripts = documents.Select(x => x.Domain).Distinct().ToDictionary(
                domain=>domain,
                domain =>  GetCompiledScript(GetScript(GetScriptName(domain))));

            foreach (var document in documents)
            {
                yield return Process(document, scripts[document.Domain]);
            }
        }

        public Script GetScript(string name)
        {
            if (!_scriptCache.TryRetrieve(name, out Script script)) { 
                lock(_compiledScriptCache)
                    { 
                    var freshScript = _scriptRepository.GetScriptByName(name) 
                        ?? HandleMissingScript(name);
                    _scriptCache.Store(name, freshScript);
                    return freshScript;
                }
            }
            return script;
        }

        public void SaveScript(Script script)
        {
            _scriptRepository.SaveOrUpdate(script);
            _scriptCache.Remove(script.Name);
        }

        public bool CanInterpret(Script script, out string errorMessage)
        {
            try
            {
                InterpretScript(script);
                errorMessage = string.Empty;
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.ToString();
                return false;
            }
        }

        private Script HandleMissingScript(string name)
        {
            if (!_settings.GenerateDefaultScript)
                throw new ScriptMissingException(name);
            Log.Warning($"No script named {name} found. Generating default and persisting it.");
            return new Script()
            {
                DomainName = name,
                Name = name,
                IsPublic = true,
                LastUpdated = DateTime.UtcNow,
                ImportedScripts = new List<Script>(),
                Metadata = "",
                Owner = "",
                Code = ResourceUtils.GetManifestString("DefaultScript.cs").Replace("[SCRIPTNAME]", name)
                
            };
        }

        private IDocument Process(IDocument document, IScript compiledScript)
        {
            if (document is IgnoreDocument)
                return document;
            if (document is AddDocument)
                return compiledScript.ProcessDocument((AddDocument)document, _utilService);
            if (document is DeleteDocument)
                return compiledScript.ProcessDelete((DeleteDocument) document, _utilService);
            throw new NotSupportedException($"No support for IDocument of type {document.GetType().Name}");
        }

        private string GetScriptName(string domain)
        {
            return domain;
        }
        private IScript GetCompiledScript(Script script)
        {
            if (!_compiledScriptCache.TryGetValue(script.Name, out ScriptWrapper cachedScript)
                || cachedScript.Script.LastUpdated <= script.LastUpdated) //recompile the script
            {
                var compiledScript = InterpretScript(script);
                _compiledScriptCache[script.Name] = new ScriptWrapper(script, compiledScript);
                return compiledScript;
            }
            return cachedScript.InterpretedScript;
        }

        private IScript InterpretScript(Script script)
        {
            var option = ScriptOptions.Default
                .WithReferences(typeof(IScript).GetTypeInfo().Assembly);
            try
            {
                var createdScript = CSharpScript.Create(script.Code, option)
                    .ContinueWith<IScript>(
                        $"new {script.Name}() as IScript")
                    .CreateDelegate()
                    .Invoke()
                    .Result;
                return createdScript;
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error when interpreting script {script.Name}: {e.Message}");
                throw;
            }
        }
        

        


        private class ScriptWrapper
        {
            public ScriptWrapper(Script script, IScript interpretedScript)
            {
                Script = script;
                InterpretedScript = interpretedScript;
                
            }

            public Script Script { get; private set; }
            public IScript InterpretedScript { get; private set; }
            
        }
    }
}
