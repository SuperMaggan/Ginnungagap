using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Bifrost.Core.Utils
{
    /// <summary>
    /// Helper class for handling xml data
    /// </summary>
    public static class XmlUtils
    {
        private static readonly Regex HexadecRegex = new Regex("[\x00-\x08\x0B\x0C\x0E-\x1F]", RegexOptions.Compiled);
        public static string ReplaceHexadecimalSymbols(string txt)
        {
            return HexadecRegex.Replace(txt, "");
        }

        private static readonly Regex ElementHexadecRegex = new Regex("[\x00-\x2f\xA4-\xBF]", RegexOptions.Compiled);
    
        public static string ReplaceInvalidElementNameChars(string elementName)
        {
            return ElementHexadecRegex.Replace(elementName, "");
        }

        public static XElement ToXElement(this object obj, Type type)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var xmlSerializer = new XmlSerializer(type);
                    xmlSerializer.Serialize(streamWriter, obj);
                    return XElement.Parse(Encoding.UTF8.GetString(memoryStream.ToArray()));
                }
            }
        }
        public static XElement ToXElement<T>(this object obj)
        {
            return obj.ToXElement(typeof (T));
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
