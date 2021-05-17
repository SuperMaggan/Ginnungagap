using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain.TextExtraction;
using Serilog;

namespace Bifrost.Common.ApplicationServices.Tika
{
    /// <summary>
    /// A parallel implementation of a text extraction service
    /// </summary>
    public class ParallellTextExtractionService : ITextExtractionService
    {
        private readonly ITextExtractor[] _extractors;


            
        /// <summary>
        /// Give the TextExtractors that should handle binary data
        /// </summary>
        /// <param name="extractors"></param>
        public ParallellTextExtractionService(ITextExtractor[] extractors)
        {
            _extractors = extractors;
            
            Log.Information(string.Format("Added {0} text extractors to {1}", _extractors.Length, this.GetType().Name));
            foreach (var textExtractor in extractors)
            {
                Log.Information(string.Format("--> {0}", textExtractor.GetType().Name));
            }
        }

        public IEnumerable<ExtractedDocument> ExtractText(IList<BinaryDocumentFile> documentFiles)
        {
            var result = new ConcurrentBag<ExtractedDocument>();
            Parallel.ForEach(documentFiles, 
                file =>
            {
                try
                {
                    result.Add(
                        new ExtractedDocument(
                            GetTextExtractor(file).ExtractText(file)));
                }

                catch (TextExtractionException e)
                {
                    Log.Error(e,$"Error when extracting file {file.Id}: {e.Message}");
                    result.Add(new ExtractedDocument(e));
                }
            });
            return result;
        }

        public IEnumerable<ExtractedDocument> ExtractText(IList<BinaryDocumentStream> documentStreams)
        {

            var result = new ConcurrentBag<ExtractedDocument>();
            Parallel.ForEach(documentStreams,
                stream =>
                {
                    try
                    {
                        result.Add(new ExtractedDocument(GetTextExtractor(stream).ExtractText(stream)));
                    }
                    catch (TextExtractionException e)
                    {

                        Log.Error(e,$"Error when extracting stream {stream.Id}: {e.Message}");
                        result.Add(new ExtractedDocument(e));
                    }
                });
            return result;
        }

        public ExtractedDocument ExtractText(BinaryDocumentFile documentFile)
        {
            try
            {
                return new ExtractedDocument(
                        GetTextExtractor(documentFile).ExtractText(documentFile));
            }
            catch (TextExtractionException e)
            {
                return new ExtractedDocument(e);
            }
        }

        public ExtractedDocument ExtractText(BinaryDocumentStream documentStream)
        {
            try
            {
                return new ExtractedDocument(GetTextExtractor(documentStream).ExtractText(documentStream));
            }
            catch (TextExtractionException e)
            {
                return new ExtractedDocument(e);
            }
        }

        private ITextExtractor GetTextExtractor(BinaryDocumentFile documentFile)
        {
            var extractor = _extractors.FirstOrDefault(x => x.CanHandle(""));
            if (extractor == null)
                throw new TextExtractionException(documentFile.Id, "No extractor was found for document stream " + documentFile.Id);
            return extractor;
        }
        private ITextExtractor GetTextExtractor(BinaryDocumentStream documentFile)
        {
            var extractor = _extractors.FirstOrDefault(x => x.CanHandle(""));
            if (extractor == null)
                throw new TextExtractionException(documentFile.Id, "No extractor was found for document stream " + documentFile.Id);
            return extractor;
        }
    
    }
}