namespace Bifrost.Core.Connectors.Configs.SqlDatabase
{
    //[Serializable]
    public class TableDetail
    {
        /// <summary>
        /// Table or view
        /// </summary>
        public string TableName { get; set; }

        public string PrimaryKeyName { get; set; }

        public bool PrimaryKeyIsInteger { get; set; }
    }
}
