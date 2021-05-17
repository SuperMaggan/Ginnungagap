//namespace Bifrost.Core.Domain.Document
//{
//    /// <summary>
//    /// A delete document will be treated as an Delete operation -> Delete the document with the given Id
//    /// </summary>
//    public class DeleteDocument : DocumentBase
//    {
//        public DeleteDocument(string idFieldName, string id):base(id)
//        {
//            IdFieldName = idFieldName;
//        }

//        public string IdFieldName { get; set; }
//        public override DocumentBase Clone()
//        {
//            return new DeleteDocument(IdFieldName, Id);
//        }
//    }
//}