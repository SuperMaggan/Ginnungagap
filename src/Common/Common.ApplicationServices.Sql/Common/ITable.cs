namespace Bifrost.Common.ApplicationServices.Sql.Common
{
    /// <summary>
    ///     Big ugly work around, the TableName MUST be the same as the property named
    ///     used in the Database implementation
    /// </summary>
    public interface ITable
    {
        string TableName { get; }
        string GetCreateTableSql();
    }
}