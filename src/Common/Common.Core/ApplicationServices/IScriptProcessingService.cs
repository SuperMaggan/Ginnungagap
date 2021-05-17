using System.Collections.Generic;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Common.Core.ApplicationServices
{
    /// <summary>
    /// Provides an interface for processing documents using different script languages, eg. CS script or Python
    /// </summary>
    public interface IScriptProcessingService
    {
        /// <summary>
        /// Processes the given document with the script that is used for the document's domain
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        IDocument Process(IDocument document);

        /// <summary>
        /// Processes the document with the given Script
        /// </summary>
        /// <param name="document"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        IDocument Process(IDocument document, Script script);

        IEnumerable<IDocument> Process(IList<IDocument> documents);

        /// <summary>
        /// Returns the script with the given name
        /// </summary>
        /// <returns></returns>
        Script GetScript(string name);

        void SaveScript(Script script);

        /// <summary>
        /// tries to Interpret the given script
        /// If an error occours, the errorMessage variable will contain a descriptive text
        /// </summary>
        /// <param name="script"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool CanInterpret(Script script, out string errorMessage);
    }
}