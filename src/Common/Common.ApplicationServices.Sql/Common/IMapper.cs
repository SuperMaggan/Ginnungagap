namespace Bifrost.Common.ApplicationServices.Sql.Common
{
    public interface IMapper<TFrom, TTo>
    {
        TTo Map(TFrom sourceObject);
        TFrom MapBack(TTo sourceObject);
    }
}