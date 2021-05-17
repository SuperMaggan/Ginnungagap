using System;
using System.Text;

namespace Bifrost.Common.Core.Domain
{
    /// <summary>
    /// Provides an interface for a document structure
    /// </summary>
    public interface IDocument
    {
        /// <summary>
        /// Identifier used to uniquely find this document
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Which domain this specific documents belongs to
        /// </summary>
        string Domain { get; set; }


        /// <summary>
        /// Creates a deep clone of the current object
        /// </summary>
        /// <returns></returns>
        IDocument Clone();
    }

   
}
