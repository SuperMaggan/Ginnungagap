using System;
using System.Globalization;
using System.Reflection;
using Bifrost.Common.Core.Settings;

namespace Bifrost.Common.Core.Helpers
{
    public static class ReflectionHelper
    {
        /// <summary>
        ///     Sets the given objects property with the string value
        ///     The prop.PropertyType decides how strValue should be parsed
        /// </summary>
        /// <param name="objectToSet"></param>
        /// <param name="prop"></param>
        /// <param name="strValue"></param>
        public static void SetProperty(object objectToSet, PropertyInfo prop, string strValue)
        {
            if (prop == null)
                return;

            if (prop.PropertyType == typeof (ConnectionStringSettings))
            {
                prop.SetValue(objectToSet, new ConnectionStringSettings(){ConnectionString = strValue}, null);
            }
            else if (prop.PropertyType.GetTypeInfo().IsEnum)
            {
                var enumVal = Enum.Parse(prop.PropertyType, strValue);
                prop.SetValue(objectToSet, (int) enumVal, null);
            }
            else if (prop.PropertyType == typeof (DateTime?))
            {
                if (string.IsNullOrEmpty(strValue))
                    prop.SetValue(objectToSet, null, null);
                else
                    prop.SetValue(objectToSet, DateTime.Parse(strValue, CultureInfo.InvariantCulture), null);
            }
            else if (string.IsNullOrEmpty(strValue))
                prop.SetValue(objectToSet, GetDefault(prop.PropertyType), null);
            else
                prop.SetValue(objectToSet, Convert.ChangeType(strValue, prop.PropertyType, CultureInfo.InvariantCulture),
                    null);
        }

        public static object GetDefault(Type type)
        {
            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }

        public static T GetPropertyValue<T>(object instance, string propertyName) where T : class
        {
            return instance.GetType().GetTypeInfo().GetProperty(propertyName).GetValue(instance, null) as T;
        }
        

        ///// <summary>
        /////     Used if the value to set is an connectionstringsettings
        ///// </summary>
        ///// <param name="objectToSet"></param>
        ///// <param name="prop"></param>
        ///// <param name="connectionStringSettings"></param>
        //public static void SetProperty(object objectToSet, PropertyInfo prop,
        //    ConnectionString connectionStringSettings)
        //{
        //    if (prop == null)
        //        return;

        //    if (prop.PropertyType == typeof (ConnectionString))
        //    {
        //        prop.SetValue(objectToSet, connectionStringSettings, null);
        //    }
        //}
    }
}