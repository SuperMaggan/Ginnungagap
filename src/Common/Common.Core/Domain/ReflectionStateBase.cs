using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bifrost.Common.Core.Helpers;

namespace Bifrost.Common.Core.Domain
{
    /// <summary>
    ///     Used as a base class for resolving a State's properties from the list of fields
    /// </summary>
    public abstract class ReflectionStateBase : State
    {
        /// <summary>
        ///     Fields that are not defined as a property
        /// </summary>
        private readonly IList<Field> _customFields = new List<Field>();

        public override IList<Field> Fields
        {
            get => GetFields();
            set => SetFields(value);
        }

        private IList<Field> GetFields()
        {
            var properties = GetType().GetTypeInfo().GetProperties();
            var list = new List<Field>(properties.Length);
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.Name == "Fields" || propertyInfo.Name == "Name")
                    continue;

                var value = propertyInfo.GetValue(this, null);
                if (value != null)
                    list.Add(new Field(propertyInfo.Name, value.ToString()));
            }
            list.AddRange(_customFields);
            return list;
        }

        private void SetFields(IEnumerable<Field> fields)
        {
            foreach (var field in fields)
            {
                var prop = GetType().GetTypeInfo().GetProperty(field.Name);
                if (prop == null)
                    _customFields.Add(new Field(field.Name, field.Value));
                else if (prop.Name == "Name")
                    continue;

                ReflectionHelper.SetProperty(this, prop, field.Value);
            }
        }

        /// <summary>
        ///     Sets the field with the name fieldName
        ///     (Note that only custom fields, that is, fields not derived from properties, will be changed
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public override void SetValue(string fieldName, string value)
        {
            var field =
                _customFields.FirstOrDefault(x => x.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
            if (field == null)
                _customFields.Add(new Field(fieldName, value));
            else
                field.Value = value;
        }

        /// <summary>
        ///     tries to retrieve the value from one of the custom fields first, if none exist, check the fields derived from the
        ///     properties
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public override string GetValue(string fieldName)
        {
            var field =
                _customFields.FirstOrDefault(x => x.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
            var value = field?.Value;
            if (value == null)
                return base.GetValue(fieldName);
            return value;
        }
    }
}