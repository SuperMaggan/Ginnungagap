namespace Bifrost.Common.Core.Domain.DocumentProcessing
{
    /// <summary>
    /// Used for storing fields with information that eg. other documents can access
    /// </summary>
    public class KeyValueUtilObject : IProcessingUtilObject
    {
        public string Serialize()
        {
            return Value;
        }

        public void Deserialize(string key, string objectStr)
        {
            Key = key;
            Value = objectStr;
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}