using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bifrost.Common.ApplicationServices.Tika;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain;
using Bifrost.Common.Core.Domain.TextExtraction;
using Bifrost.Common.IntegrationTests.Utilities;
using FluentAssertions;
using Xunit;

namespace Bifrost.Common.IntegrationTests.Tika
{
    public class TikaServerTests
    { 
        private readonly ITextExtractionService _service;
        private const string ContentFieldName = "X-TIKA:content";
        public TikaServerTests()
        {
            var settings = new TikaSettings();
            var extractor = new TikaServerTextExtractor(settings);
            _service = new ParallellTextExtractionService(new ITextExtractor[] { extractor });
        }

        

        [Fact]
        public void CanExtractPdfFromStream()
        {

            using (var stream = ResourceUtils.GetManifestResourceStream("Testdata.Binary.test.pdf"))
            {
                long processTime;
                var binaryStream = new BinaryDocumentStream("test.pdf","test",stream);
                var doc = ExtractStream(binaryStream, out processTime);
                Console.WriteLine("{0} extraction time (ms):{1}", "test.pdf", processTime);
                doc.Domain.Should().Be("test");

                doc.Fields.Should().NotBeNull();
                doc.Fields.Count.Should().BeGreaterThan(10);
                doc.GetFieldValue(ContentFieldName).Should().Contain("Ombyggnation till Mötesplats");
            }
        }



       

        [Fact]
        public void CanExtractPowerpointFromStream()
        {
            using (var stream = ResourceUtils.GetManifestResourceStream("Testdata.Binary.Findwise presentation - Hades.pptx"))
            {
                var binaryStream = new BinaryDocumentStream("Findwise presentation - Hades.pptx","test", stream);
                long processTime;
                var doc = ExtractStream(binaryStream, out processTime);
                Console.WriteLine("{0} extraction time (ms):{1}", "Findwise presentation - Hades.pptx", processTime);
                doc.Domain.Should().Be("test");
                doc.Fields.Should().NotBeNull();
                doc.Fields.Count.Should().BeGreaterThan(10);
                doc.GetFieldValue(ContentFieldName).Should().Contain("need to be able to reindex");
            }
        }

        [Fact]
        public void CanExtractMacromanPowerpointFromStream()
        {
            using (var stream = ResourceUtils.GetManifestResourceStream("Testdata.Binary.Presentation1.ppt"))
            {
                var binaryStream= new BinaryDocumentStream("Presentation1.ppt","test", stream);
                long processTime;
                var doc = ExtractStream(binaryStream, out processTime);
                Console.WriteLine("{0} extraction time (ms):{1}", "Presentation1.ppt", processTime);
                doc.Domain.Should().Be("test");
                doc.Fields.Should().NotBeNull();
                doc.Fields.Count.Should().BeGreaterThan(10);
                doc.GetFieldValue(ContentFieldName).Should().Contain("PowerPoint Presentation");
            }
        }

        [Fact]
        public void CanExtractImagedPowerpointFromStream()
        {
            using (var stream = ResourceUtils.GetManifestResourceStream("Testdata.Binary.Presentation with image.ppt"))
            {
                var binaryStream = new BinaryDocumentStream("Presentation with image.ppt","test", stream);
                long processTime;
                var doc = ExtractStream(binaryStream, out processTime);
                Console.WriteLine("{0} extraction time (ms):{1}", "Presentation with image.ppt", processTime);
                doc.Domain.Should().Be("test");
                doc.Fields.Should().NotBeNull();
                doc.Fields.Count.Should().BeGreaterThan(10);
                doc.GetFieldValue(ContentFieldName).Should().Contain("Creuna");
            }
        }
        [Fact]
        public void CanExtractDocxFromStream()
        {
            using (var stream = ResourceUtils.GetManifestResourceStream("Testdata.Binary.Hades.docx"))
            {
                var binaryStream = new BinaryDocumentStream("Hades.docx","test", stream);
                long processTime;
                var doc = ExtractStream(binaryStream, out processTime);
                Console.WriteLine("{0} extraction time (ms):{1}", "Hades.docx", processTime);
                doc.Domain.Should().Be("test");
                doc.Fields.Should().NotBeNull();
                doc.Fields.Count.Should().BeGreaterThan(10);
                doc.GetFieldValue(ContentFieldName).Should().Contain("Hades uses Quartz for");

            }

        }
        [Fact]
        public void CanExtractPngFromStream()
        {
            using (var stream = ResourceUtils.GetManifestResourceStream("Testdata.Binary.tux.png"))
            {
                var binaryStream = new BinaryDocumentStream("tux.png","test", stream);
                long processTime;
                var doc = ExtractStream(binaryStream, out processTime);
                Console.WriteLine("{0} extraction time (ms):{1}", "tux.png", processTime);
                doc.Domain.Should().Be("test");
                doc.Fields.Should().NotBeNull();
                doc.Fields.Count.Should().BeGreaterThan(10);
                doc.Fields.FirstOrDefault(x => x.Name == "width" && x.Value == "400").Should().NotBeNull();

            }

        }
        [Fact]
        public void CanExtractPowerpointFromFile()
        {
            var dir = Directory.GetCurrentDirectory();
            var filePath = @"Testdata\Binary\Findwise presentation - Hades.pptx";
            var absolutePath = Path.Combine(dir, filePath);
            var file = new BinaryDocumentFile(absolutePath, "test", absolutePath);
            long processTime;
            var doc = ExtractFile(file, out processTime);
            Console.WriteLine("{0} extraction time (ms):{1}", filePath, processTime);
            doc.Domain.Should().Be("test");
            doc.Fields.Should().NotBeNull();
            doc.Fields.Count.Should().BeGreaterThan(10);
            doc.GetFieldValue(ContentFieldName).Should().Contain("need to be able to reindex");
        }

        [Fact]
        public void CanExtractDocxFromFile()
        {
            var filePath = @"Testdata\Binary\Hades.docx";
            var file = new BinaryDocumentFile(filePath, "test", filePath);
            long processTime;
            var doc = ExtractFile(file, out processTime);
            Console.WriteLine("{0} extraction time (ms):{1}", filePath, processTime);
            doc.Domain.Should().Be("test");
            doc.Fields.Should().NotBeNull();
            doc.Fields.Count.Should().BeGreaterThan(10);
            doc.GetFieldValue(ContentFieldName).Should().Contain("Hades uses Quartz for");
            
        }

     

        [Fact]
        public void CanExtractPngFromFile()
        {
            var dir = Directory.GetCurrentDirectory();
            var filePath = @"Testdata\Binary\tux.png";
            var absolutePath = Path.Combine(dir, filePath);
            var file = new BinaryDocumentFile(absolutePath, "test", absolutePath);
            long processTime;
            var doc = ExtractFile(file, out processTime);
            Console.WriteLine("{0} extraction time (ms):{1}", filePath, processTime);

            doc.Domain.Should().Be("test");
            doc.Fields.Should().NotBeNull();
            doc.Fields.Count.Should().BeGreaterThan(10);
            doc.Fields.FirstOrDefault(x => x.Name == "width" && x.Value == "400").Should().NotBeNull();
            

        }

       

        [Fact]
        public void CanExtractJpgFromFile()
        {
            var dir = Directory.GetCurrentDirectory();
            var filePath = @"Testdata\Binary\Styx architecture.jpg";
            var absolutePath = Path.Combine(dir, filePath);
            var file = new BinaryDocumentFile(absolutePath, "test", absolutePath);
            long processTime;
            var doc = ExtractFile(file, out processTime);
            Console.WriteLine("{0} extraction time (ms):{1}", filePath, processTime);
            doc.Domain.Should().Be("test");
            doc.Fields.Count.Should().BeGreaterThan(1);

        }

        [Fact]
        public void CanExtractJpgFromStream()
        {
            using (var stream = ResourceUtils.GetManifestResourceStream("Testdata.Binary.Styx architecture.jpg"))
            {
                var binaryStream = new BinaryDocumentStream("Styx architecture.jpg","test", stream);
                long processTime;
                var doc = ExtractStream(binaryStream, out processTime);
                Console.WriteLine("{0} extraction time (ms):{1}", "Styx architecture.jpg", processTime);
                doc.Fields.Count.Should().BeGreaterThan(10);
                doc.Domain.Should().Be("test");
            }
        }

        [Fact]
        public void CanExtractVisioFromFile()
        {
            var dir = Directory.GetCurrentDirectory();
            var filePath = @"Testdata\Binary\.Net search architecture.vsd";
            var absolutePath = Path.Combine(dir, filePath);
            var file = new BinaryDocumentFile(absolutePath, "test", absolutePath);
            
            long processTime;
            var doc = ExtractFile(file, out processTime);
            Console.WriteLine("{0} extraction time (ms):{1}", filePath, processTime);
            doc.Domain.Should().Be("test");
            doc.Fields.Should().NotBeNull();
            doc.Fields.Count.Should().BeGreaterThan(10);
            doc.GetFieldValue(ContentFieldName).Should().Contain("Hades.AdminUI");

        }

        [Fact]
        public void CanExtractVisioFromStream()
        {
            using (var stream = ResourceUtils.GetManifestResourceStream("Testdata.Binary..Net search architecture.vsd"))
            {
                var binaryStream = new BinaryDocumentStream(".Net search architecture.vsd","test", stream);
                long processTime;
                var doc = ExtractStream(binaryStream, out processTime);
                Console.WriteLine("{0} extraction time (ms):{1}", ".Net search architecture.vsd", processTime);
                doc.Domain.Should().Be("test");
                doc.Fields.Should().NotBeNull();
                doc.Fields.Count.Should().BeGreaterThan(10);
                doc.GetFieldValue(ContentFieldName).Should().Contain("Hades.AdminUI");

            }

        }



        private AddDocument ExtractFile(BinaryDocumentFile file, out long processTime)
        {
            var timer = new Stopwatch();
            timer.Start();
            var doc = _service.ExtractText(new[] { file }).FirstOrDefault();
            timer.Stop();
            processTime = timer.ElapsedMilliseconds;
            if (doc.TextExtractionException != null)
                throw doc.TextExtractionException;
            return doc.ResultingDocument as AddDocument;
        }

        private AddDocument ExtractStream(BinaryDocumentStream stream, out long processTime)
        {
            var timer = new Stopwatch();
            timer.Start();
            var doc = _service.ExtractText(new[] { stream }).FirstOrDefault();
            timer.Stop();
            processTime = timer.ElapsedMilliseconds;
            if (doc.TextExtractionException != null)
                throw doc.TextExtractionException;
            return doc.ResultingDocument as AddDocument; ;
        }
    }
}
