//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Bifrost.Common.Core.Domain;

//namespace Bifrost.Core.Domain.Document
//{
//    public class AddDocument :DocumentBase
//    {
//        public AddDocument() : this("")
//        {}
//        public AddDocument(string id)
//            :base(id)
//        {   
//            Fields = new List<Field>();
//        }

//        public IList<Field> Fields { get; set; }

//        /// <summary>
//        /// Gets the field with the given fieldName                          n
//        /// If field does not exist, null will be returned
//        /// </summary>
//        /// <param name="fieldName">Name of field</param>
//        /// <param name="caseSensitive">Must match case?</param>
//        /// <returns>Field or null</returns>
//        public Field GetField(string fieldName, bool caseSensitive=true)
//        {
//            return caseSensitive ? 
//                Fields.FirstOrDefault(x => x.Name.Equals(fieldName)) : 
//                Fields.FirstOrDefault(x => x.Name.Equals(fieldName, StringComparison.CurrentCultureIgnoreCase));
//        }


//        public override DocumentBase Clone()
//        {
//            var doc = new AddDocument(this.Id) { Fields = new List<Field>() };
//            if (Fields == null) return doc;
//            foreach (var field in Fields)
//            {
//                doc.Fields.Add((Field)field.Clone());
//            }
//            return doc;
//        }
//    }


//}
