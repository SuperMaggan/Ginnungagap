using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bifrost.Core.Domain.Document;
using Bifrost.Core.Pipeline.Interfaces;
using Bifrost.Core.Settings;
using Bifrost.Common.Core.Domain;
using Serilog;

namespace Bifrost.Core.Pipeline.DocumentSteps
{
    public class LimitDocumentSizePipelineStep : IDocumentPipelineStep
    {
        private readonly CommonSettings _commonSettings;

      public LimitDocumentSizePipelineStep(CommonSettings commonSettings)
        {
            _commonSettings = commonSettings;
            if (_commonSettings.MaxFieldSizeKb > 0)
                Enabled = true;
        }

        /// <summary>
        /// Invoke the step rule on the given document
        /// </summary>
        /// <param name="document"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public IDocument Invoke(IDocument document, string domain)
        {
            var maxBytesCount = _commonSettings.MaxFieldSizeKb*1024;
            if (!(document is AddDocument)) return document;
            var newFields = new List<Field>();
            foreach (var field in (document as AddDocument).Fields)
            {
                var byteCount = Encoding.UTF8.GetByteCount(field.Value);

                if (field is BinaryField)
                {
                    var binaryField = field as BinaryField;
                    if (binaryField.Data.Length > maxBytesCount)
                    {   
                        binaryField.Value = $"Removed binary data ({binaryField.Data.Length}bytes) due to being longer than {maxBytesCount}bytes";
                        binaryField.Data = new byte[0];
                        Log.Debug($"Removed binary field '{field.Name}' in {document.Id} " +
                                     $"due to being larger ({binaryField.Data.Length}bytes) than {maxBytesCount}bytes");
                    }
                    newFields.Add(new Field($"{field.Name}_sizeBytes",byteCount.ToString()));
                }
                else if (byteCount  > maxBytesCount)
                {
                    //most UTF8 encoded chars are 2bytes, lets assume this (no point in being accurate)
                    var numChars = field.Value.Length > maxBytesCount/2
                        ? maxBytesCount/2
                        : field.Value.Length;
                    field.Value = $"{field.Value.Substring(0, numChars)}...[truncated by Asgard ({byteCount}bytes)]";
                    Log.Debug($"Truncated field '{field.Name}' in {document.Id} to {numChars} characters");
                }

            }
            if (!newFields.Any())
                return document;
            foreach (var newField in newFields)
            {
                (document as AddDocument).Fields.Add(newField);   
            }
            return document;

        }

        public bool Enabled { get; set; }

        public override string ToString()
        {
            return $"{this.GetType().Name}: Enabled ({Enabled}). Truncates fields containing more than {_commonSettings.MaxFieldSizeKb}kB";
        }
    }
}