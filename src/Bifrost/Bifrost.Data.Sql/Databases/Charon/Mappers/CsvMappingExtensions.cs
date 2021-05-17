using System;
using System.Collections.Generic;
using System.Linq;

namespace Bifrost.Data.Sql.Databases.Bifrost.Mappers
{
    public static class CsvMappingExtensions
    {
        private const string Delimiter = " <;> ";

        /// <summary>
        /// Takes the delimiter seperated string and creates an array of the values
        /// </summary>
        /// <param name="csvString"></param>
        /// <returns></returns>
        public  static IList<string> ToCsvArray(this string csvString)
        {
            if (string.IsNullOrEmpty(csvString))
                return new List<string>(0);
            var tokens = csvString.Split(new[] { Delimiter }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return tokens;
        }

        /// <summary>
        /// Creates a delimiter seperated string of the given string
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ToCsvString(this IEnumerable<string> array)
        {
            if (array == null)
                return "";
            return string.Join(Delimiter, array);
        }

    }
}