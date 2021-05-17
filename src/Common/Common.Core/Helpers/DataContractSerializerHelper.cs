using System;
using System.Xml.Linq;
//using System.Runtime.Serialization;

namespace Bifrost.Common.Core.Helpers
{
    public static class DataContractSerializerHelper
    {

        /// <summary>
        /// Serialize the given obj to XElement using the DataContractSerializer
        /// Use this if your class is decorated with [DataContract], [DataMember] etc
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static XElement ToXElementUsinDataContract(this object obj, Type type)
        {
            throw new NotImplementedException();
            //using (var memoryStream = new MemoryStream())
            //{
            //    DataContractSerializer serializer = new DataContractSerializer(type);
            //    serializer.WriteObject(memoryStream, obj);
            //    memoryStream.Flush();
            //    return XElement.Parse(Encoding.UTF8.GetString(memoryStream.ToArray()));
            //}
        }
        /// <summary>
        /// Serialize the given obj to XElement using the DataContractSerializer
        /// Use this if your class is decorated with [DataContract], [DataMember] etc
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static XElement ToXElementUsinDataContract<T>(this object obj)
        {
            return obj.ToXElementUsinDataContract(typeof(T));
        }


        /// <summary>
        /// Deserialize the given XElement to an object using the DataContractSerializer
        /// Use this if the XElement is serialized using DataContractSerializer 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xElement"></param>
        /// <returns></returns>
        public static T FromXElementUsinDataContract<T>(this XElement xElement)
        {
            throw new NotImplementedException();
            //DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            //return (T)serializer.ReadObject(xElement.CreateReader());
        }

        /// <summary>
        /// Deserialize the given XElement to an object using the DataContractSerializer
        /// Use this if the XElement is serialized using DataContractSerializer 
        /// </summary>
        /// <param name="xElement"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object FromXElementUsinDataContract(this XElement xElement, Type type)
        {
            throw new NotImplementedException();
            //DataContractSerializer serializer = new DataContractSerializer(type);
            //return serializer.ReadObject(xElement.CreateReader());
        }

    }
}