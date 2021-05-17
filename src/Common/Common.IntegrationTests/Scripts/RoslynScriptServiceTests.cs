using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bifrost.Common.ApplicationServices.Roslyn;
using Bifrost.Common.ApplicationServices.Sql;
using Bifrost.Common.ApplicationServices.Sql.Common;
using Bifrost.Common.ApplicationServices.Sql.Databases;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Data;
using Bifrost.Common.Core.Domain;
using Bifrost.Common.IntegrationTests.Utilities;
using FluentAssertions;
using Xunit;

namespace Bifrost.Common.IntegrationTests.Scripts
{
    public class RoslynScriptServiceTests
    {
        IScriptProcessingService CreateService()
        {
            return new RoslynScriptProcessingService(
                new SqlScriptRepository(new ScriptDatabase(new CommonDatabaseSettings())),
                new SqlProcessingUtilService(new ProcessingUtilDatabase(new CommonDatabaseSettings())),
                new ScriptProcessingSettings()
            );
            
        }

        [Fact]
        public void Can_interpret_regexscript()
        {
            var service = CreateService();

            var script = new Script()
            {
                Code = ResourceUtils.GetManifestString("Scripts.Data.RegexScript.cs"),
                Name = "RegexScript"
            };

            var canInterpret = service.CanInterpret(script, out string errorMsg);
            canInterpret.Should().BeTrue($"Failed interpreting script: " + errorMsg);
        }

        private IDocument CreateAddDocument(string  domain = "kite")
        {
            return new AddDocument()
            {
                Id = "kite4life",
                Domain = domain,
                Fields = new List<Field>()
                {
                    new Field("body", "This wont work, magnus"),
                    new Field("description", "You bet it wont")
                }
            };
        }

        private IDocument CreateDeleteDocument()
        {
            return new DeleteDocument("kite4life","kite");
        }
        

    }
}

