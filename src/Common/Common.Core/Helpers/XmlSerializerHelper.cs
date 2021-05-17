using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Bifrost.Common.Core.Helpers
{
    /// <summary>
    /// Static extension methods using XmlSerializer to serialize/deserialize xml
    /// </summary>
    public static class XmlSerializerHelper
    {

        public static XElement ToXElement(this object obj, Type type)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    var xmlSerializer = new XmlSerializer(type);
                    xmlSerializer.Serialize(streamWriter, obj);
                    return XElement.Parse(Encoding.UTF8.GetString(memoryStream.ToArray()));
                }
            }
        }
        public static XElement ToXElement<T>(this object obj)
        {
            return obj.ToXElement(typeof(T));
        }

        public static T FromXElement<T>(this XElement xElement)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(xElement.CreateReader());
        }

        public static object FromXElement(this XElement xElement, Type type)
        {
            var serializer = new XmlSerializer(type);
            return serializer.Deserialize(xElement.CreateReader());
        }

    }
}