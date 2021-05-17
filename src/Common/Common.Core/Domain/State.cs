using System;
using System.Collections.Generic;
using System.Linq;

namespace Bifrost.Common.Core.Domain
{
    /// <summary>
    ///     A generic state class
    /// </summary>
    public class State
    {
        /// <summary>
        ///     Key/name of this state
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Current states
        /// </summary>
        public virtual IList<Field> Fields { get; set; }

        /// <summary>
        ///     Will try to find the value of the named field
        /// </summary>
        /// <param name="fieldName">Case insensitive field name (default)</param>
        /// <returns>value of the corresponding field. Null if not found</returns>
        public virtual string GetValue(string fieldName)
        {
            var field = Fields.FirstOrDefault(x => x.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
            return field == null ? null : field.Value;
        }

        /// <summary>
        ///     Set a field value
        ///     If the field already exist the value will be replaces
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public virtual void SetValue(string fieldName, string value)
        {
            var field = Fields.FirstOrDefault(x => x.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
            if (field == null)
                Fields.Add(new Field(fieldName, value));
            else
                field.Value = value;
        }
    }
}