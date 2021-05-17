using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Bifrost.Common.ApplicationServices.Roslyn;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Data;
using Bifrost.Common.Core.Domain;
using Bifrost.Common.Core.Settings;
using Bifrost.Common.Tests.Utilities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Bifrost.Common.Tests.Scripts
{
    public class RoslynScriptServiceTests
    {

        IScriptProcessingService CreateService()
        {
            var script = new Script()
            {
                Code = ResourceUtils.GetManifestString("Scripts.Data.RegexScript.cs"),
                Name = "RegexScript"
            };
            var scriptRepo = new Mock<IScriptRepository>();
            scriptRepo
                .Setup(x => x.GetScriptByName(It.IsAny<string>()))
                .Returns(() => script);
            
            var utilService = new Mock<IProcessingUtilService>();
            return new RoslynScriptProcessingService(scriptRepo.Object, utilService.Object, new ScriptProcessingSettings());
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

        [Fact]
        public void Will_fail_when_interpreting_brokenscript()
        {
            var service = CreateService();

            var script = new Script()
            {
                Code = ResourceUtils.GetManifestString("Scripts.Data.BrokenScript.cs"),
                Name = "RegexScript"
            };

            var canInterpret = service.CanInterpret(script, out string errorMsg);
            canInterpret.Should().BeFalse();
            errorMsg.Should().NotBeEmpty();
        }

        [Fact]
        public void Can_process_add_with_regexscript()
        {
            var service = CreateService();

            var script = new Script()
            {
                Code = ResourceUtils.GetManifestString("Scripts.Data.RegexScript.cs"),
                Name = "RegexScript"
            };

            var doc = CreateAddDocument();
            var processed = service.Process(doc, script) as AddDocument;

            processed.Should().NotBeNull();
            processed.Id.Should().Be(doc.Id);
            processed.Domain.Should().Be(doc.Domain);
            processed.GetFieldValue("magnus").Should().Be("magnus");
        }

        [Fact]
        public void Can_process_delete_with_regexscript()
        {
            var service = CreateService();

            var script = new Script()
            {
                Code = ResourceUtils.GetManifestString("Scripts.Data.RegexScript.cs"),
                Name = "RegexScript"
            };

            var doc = CreateDeleteDocument();
            var processed = service.Process(doc, script) as DeleteDocument;
            processed.Should().NotBeNull();
            processed.Id.Should().Be(doc.Id);
            processed.Domain.Should().Be(doc.Domain);
        }

        [Fact]
        public void Will_use_cached_scripts()
        {
            var script = new Script()
            {
                Code = ResourceUtils.GetManifestString("Scripts.Data.RegexScript.cs"),
                Name = "RegexScript"
            };
            var scriptRepo = new Mock<IScriptRepository>();
            var scriptRepoGetScriptCalledNum = 0;
            scriptRepo
                .Setup(x => x.GetScriptByName(It.IsAny<string>()))
                .Returns(() => script)
                .Callback(() => scriptRepoGetScriptCalledNum++);

            var utilService = new Mock<IProcessingUtilService>();
            var service =  new RoslynScriptProcessingService(scriptRepo.Object, utilService.Object, new ScriptProcessingSettings());

            var doc = CreateAddDocument();

            var firstRun = 0L;
            for(int i = 0; i < 100; i++)
            {
                var timer = Stopwatch.StartNew();
                var processed = service.Process(doc) as AddDocument;
                if (firstRun == 0)
                    firstRun = timer.ElapsedMilliseconds*2; //for some margin..
                timer.Stop();

                processed.Should().NotBeNull();
                processed.Id.Should().Be(doc.Id);
                processed.Domain.Should().Be(doc.Domain);
                processed.GetFieldValue("magnus").Should().Be("magnus");
                scriptRepoGetScriptCalledNum.Should().Be(1);
            }
        }

        [Fact]
        public void Can_process_multiple_adds_from_different_domains()
        {
            var service = CreateService();

            var docs = new List<IDocument>();
            for (int i = 0; i < 100; i++)
            {
                docs.Add(CreateAddDocument(Guid.NewGuid().ToString()));
            }

            var processedDocs = service.Process(docs).Cast<AddDocument>();

            foreach (var processedDoc in processedDocs)
            {
                processedDoc.Should().NotBeNull();
                processedDoc.Id.Should().Be(docs.First().Id);
                processedDoc.GetFieldValue("magnus").Should().Be("magnus");
            }
        }

        [Fact]
        public void Will_create_default_script_if_missing()
        {
            var scriptRepo = new Mock<IScriptRepository>();
            var scriptRepoGetScriptCalledNum = 0;
            scriptRepo
                .Setup(x => x.GetScriptByName(It.IsAny<string>()))
                .Returns(() => null)
                .Callback(() => scriptRepoGetScriptCalledNum++);
            

            var utilService = new Mock<IProcessingUtilService>();
            var service = new RoslynScriptProcessingService(scriptRepo.Object, utilService.Object, new ScriptProcessingSettings()
            {
                GenerateDefaultScript = true
            });


            var doc = CreateAddDocument();
            var processed = service.Process(doc) as AddDocument;
            processed.Should().NotBeNull();
            processed.Id.Should().Be(doc.Id);
            processed.Domain.Should().Be(doc.Domain);
        }


        [Fact]
        public void Will_throw_exception_if_script_missing()
        {
            var scriptRepo = new Mock<IScriptRepository>();
            var scriptRepoGetScriptCalledNum = 0;
            scriptRepo
                .Setup(x => x.GetScriptByName(It.IsAny<string>()))
                .Returns(() => null)
                .Callback(() => scriptRepoGetScriptCalledNum++);


            var utilService = new Mock<IProcessingUtilService>();
            var service = new RoslynScriptProcessingService(scriptRepo.Object, utilService.Object, new ScriptProcessingSettings()
            {
                GenerateDefaultScript = false
            });



            Action invoke = () => service.Process(CreateAddDocument());
            invoke.Should().Throw<ScriptMissingException>();


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

