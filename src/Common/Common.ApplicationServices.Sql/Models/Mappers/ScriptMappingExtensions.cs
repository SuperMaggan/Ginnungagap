using System.Collections.Generic;
using System.Linq;
using Bifrost.Common.ApplicationServices.Sql.Common.Extensions;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Common.ApplicationServices.Sql.Models.Mappers
{
    public static class ScriptMappingExtensions
    {
        public static Script ToDomain(this ScriptModel model, IList<Script> importedScripts)
        {
            return new Script
            {
                Name = model.Name,
                DomainName = model.DomainName,
                Code = model.Code,
                LastUpdated = model.LastUpdated,
                ImportedScripts = importedScripts,
                IsPublic = model.IsPublic,
                Owner = model.Owner,
                Metadata = model.Metadata
            };
        }

        public static ScriptModel ToModel(this Script script)
        {
            return new ScriptModel
            {
                Name = script.Name,
                DomainName = script.DomainName,
                Code = script.Code,
                LastUpdated = script.LastUpdated,
                ImportedScriptNamesCsv = script.ImportedScripts.Select(s => s.Name.ToString()).ToCsvString(),
                IsPublic = script.IsPublic,
                Owner = script.Owner,
                Metadata = script.Metadata
                
            };
        }
    }
}