
using Bifrost.Common.Core.Domain;

namespace Bifrost.Core.Domain.Document
{
    public class BinaryField : Field
    {
        public BinaryField(string name, byte[] data) : base(name, "")
        {
            Data = data;
        }

        public byte[] Data { get; set; }
    }
}