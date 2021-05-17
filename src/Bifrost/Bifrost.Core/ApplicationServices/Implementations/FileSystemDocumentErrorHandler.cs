using System;
using System.Globalization;
using System.IO;
using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Domain.Document;
using Bifrost.Core.Settings;
using Bifrost.Common.Core.Domain;
using Bifrost.Common.Core.Domain.TextExtraction;
using Serilog;

namespace Bifrost.Core.ApplicationServices.Implementations
{
    public class FileSystemDocumentErrorHandler : FileSystemIntegrationService, IDocumentErrorHandler
    {

        public FileSystemDocumentErrorHandler(CommonSettings settings)
            : base(Path.Combine(settings.FileSystemDirectory, "Errors"))
        {

        }
        public void PersistErrorInformation(BinaryDocumentStream stream, TextExtractionException exception)
        {
            try
            {
                if(!stream.Stream.CanRead)
                    return;
                var fileName = !string.IsNullOrEmpty(stream.Id)
                    ? Path.GetFileName(stream.Id)
                    : Guid.NewGuid().ToString();
                var filePath = Path.Combine(GetCurrentAddDirectoryPath(), fileName);
                using (var fileStream = File.Create(filePath))
                {
                    stream.Stream.Seek(0, SeekOrigin.Begin);
                    stream.Stream.CopyTo(fileStream);
                }
                Log.Error(exception,String.Format("Persisting error document at {0}", filePath));
            }
            catch (Exception e)
            {
                Log.Error(e, String.Format("Error when persisting error information for stream {0}", stream.Id));
            }
        }

        public void PersistErrorInformation(BinaryDocumentFile file, TextExtractionException exception)
        {
            try
            {
                var filePath = Path.Combine(GetCurrentAddDirectoryPath(), file.Id);
                File.Copy(file.FilePath, filePath);
                Log.Error(exception, String.Format("Persisting error document at {0}", filePath));
            }
            catch (Exception e)
            {
                Log.Error(e, String.Format("Error when persisting error information for file {0}", file.Id));
            }
        }

        public void PersistErrorInformation(IDocument document, Exception exception)
        {
            throw new NotImplementedException();
        }


        private string GetCurrentAddDirectoryPath()
        {
            var path = Path.Combine(AddDirectory, GetDateTimePathString());
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        private string GetCurrentDeleteDirectoryPath()
        {
            var path = Path.Combine(DeleteDirectory, GetDateTimePathString());
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
        private string GetDateTimePathString()
        {
            DateTime now = DateTime.UtcNow;
            return string.Format("{0}_{1}_{2}_{3}",
                now.Year.ToString(CultureInfo.InvariantCulture),
                now.Month.ToString(CultureInfo.InvariantCulture),
                now.Day.ToString(CultureInfo.InvariantCulture),
                now.Hour.ToString(CultureInfo.InvariantCulture));
        }
    }

}