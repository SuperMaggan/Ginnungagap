using System.Collections.Generic;

namespace Bifrost.Common.ApplicationServices.Sql.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static string ToSqlInConstraint(this IEnumerable<int> list)
        {
            return string.Format("({0})", string.Join(",", list));
        }

        public static string ToSqlInConstraint(this IEnumerable<string> list)
        {
            return string.Format("('{0}')", string.Join("','", list));
        }
    }
}