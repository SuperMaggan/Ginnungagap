namespace Bifrost.Common.Core.Domain.DocumentProcessing
{
    /// <summary>
    /// Represents an object used by the processing scripts to enable persisiting of states
    /// and sharing data between documents
    /// </summary>
    public interface IProcessingUtilObject
    {
        /// <summary>
        /// The Id of this object
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// Serializes the object into a string       
        /// </summary>
        /// <returns></returns>
        string Serialize();

        /// <summary>
        /// Deserializes the given string and sets this object's properties
        /// </summary>
        /// <param name="objectStr"></param>
        void Deserialize(string key, string objectStr);
    }
}
