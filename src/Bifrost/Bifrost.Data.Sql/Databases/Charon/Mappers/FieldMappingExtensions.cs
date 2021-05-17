using System;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Data.Sql.Databases.Bifrost.Mappers
{
    public static class FieldMappingExtensions
    {
        private static readonly string _delimiter ="-->";

        public static string ToCsvString(this Field field)
        {
            if (field == null)
                return "";
            return $"{field.Name}{_delimiter}{field.Value}";
        }

        public static Field ToField(this string fieldStr)
        {
            if(string.IsNullOrEmpty(fieldStr))
                throw new FormatException($"Expected a field string to be of format 'name'{_delimiter}'value' (tried parsing {fieldStr})");

            var tokens = fieldStr.Split( new [] { _delimiter }, StringSplitOptions.None);
            if(tokens.Length != 2)
                throw new FormatException($"Expected a field string to be of format 'name'{_delimiter}'value' (tried parsing {fieldStr})");
            return new Field(tokens[0],tokens[1]);
        }
        
            
        
    
    }
}