using System;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Common.ApplicationServices.Sql.Models.Mappers
{
    public static class FieldMappingExtensions
    {
        private static readonly string _delimiter = "-->";

        public static string ToCsvString(this Field field)
        {
            if (field == null)
                return "";
            return string.Format("{0}{1}{2}", field.Name, _delimiter, field.Value);
        }

        public static Field ToField(this string fieldStr)
        {
            if (string.IsNullOrEmpty(fieldStr))
                throw new FormatException(
                    string.Format("Expected a field string to be of format 'name'{0}'value' (tried parsing {1})",
                        _delimiter, fieldStr));

            var tokens = fieldStr.Split(new[] {_delimiter}, StringSplitOptions.None);
            if (tokens.Length != 2)
                throw new FormatException(
                    string.Format("Expected a field string to be of format 'name'{0}'value' (tried parsing {1})",
                        _delimiter, fieldStr));
            return new Field(tokens[0], tokens[1]);
        }
    }
}