using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bifrost.Common.Core.Helpers;

namespace Bifrost.Common.Core.Settings
{
    /// <summary>
    ///     Used as a base class for settings class that should resolve property values
    ///     1. Use environmental variable value
    ///     2. Use config value
    /// </summary>
    public abstract class ConfigSettingsBase
    {
        /// <summary>
        ///     Holds information if a Property (string) was configured using the configuration file
        /// </summary>
        private Dictionary<string, Tuple<Type, SetMethod>> _propertySetDic;

        protected ConfigSettingsBase()
        {
            Init();
        }

        private void Init()
        {
            _propertySetDic = new Dictionary<string, Tuple<Type, SetMethod>>();

            foreach (var propInfo in GetType().GetTypeInfo().GetProperties())
            {
                var envVariable = Environment.GetEnvironmentVariable(propInfo.Name);
                if (envVariable != null)
                {
                    ReflectionHelper.SetProperty(this, propInfo, envVariable);
                    _propertySetDic.Add(propInfo.Name, new Tuple<Type, SetMethod>(propInfo.PropertyType, SetMethod.EnvironmentalVariable));
                }
                else if (propInfo.PropertyType == typeof(ConnectionStringSettings))
                {
                    var value = TemporaryConnectionManager.GetConnectionString(propInfo.Name);
                    if (value != null)
                    {
                        propInfo.SetValue(this, value, null);
                        _propertySetDic.Add(propInfo.Name, new Tuple<Type, SetMethod>(propInfo.PropertyType, SetMethod.Config));
                    }
                    else
                    {
                        _propertySetDic.Add(propInfo.Name, new Tuple<Type, SetMethod>(propInfo.PropertyType, SetMethod.NotSet));
                    }

                }
                else
                {
                    var value = TemporaryConnectionManager.GetAppSetting(propInfo.Name);
                    if (value != null)
                    {
                        ReflectionHelper.SetProperty(this, propInfo, value);
                        _propertySetDic.Add(propInfo.Name, new Tuple<Type, SetMethod>(propInfo.PropertyType, SetMethod.Config));
                    }
                    else
                    {
                        _propertySetDic.Add(propInfo.Name, new Tuple<Type, SetMethod>(propInfo.PropertyType, SetMethod.NotSet));
                    }
                }
            }
        }


        /// <summary>
        ///     Returns a list of strings describing the settings that has been set using the config file
        ///     format:  'SettingsName1' (bool) set to 'Value'
        ///     Use this if you want to write log messages
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<string> GetSetSettingsMessages()
        {
            foreach (var p in _propertySetDic.Where(x => x.Value.Item2 != SetMethod.NotSet))
            {
                if(p.Value.Item2 == SetMethod.EnvironmentalVariable)
                    yield return $"'{p.Key}' ({p.Value.Item1.Name}) set to '{GetPropertyValue(p.Key)}' (environmental variable)";
                if (p.Value.Item2 == SetMethod.Config)
                    yield return $"'{p.Key}' ({p.Value.Item1.Name}) set to '{GetPropertyValue(p.Key)}' (config)";
            }
        }

        /// <summary>
        ///     Returns a list of strings describing the settings that has NOT been set (no corresponding settings found in the
        ///     config file)
        ///     format: 'SettingsName2' (ConnectionSettingsString) was not found, defaulted to 'Value'
        ///     Use this if you want to write log messages
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<string> GetNotSetSettingsMessages()
        {
            foreach (var p in _propertySetDic.Where(x => x.Value.Item2 == SetMethod.NotSet))
            {
                yield return $"'{p.Key}' ({p.Value.Item1.Name}) was not found in your config, defaulted to '{GetPropertyValue(p.Key)}'";
            }
        }

        private string GetPropertyValue(string propertyName)
        {
            var val = GetType()
                .GetTypeInfo()
                .GetProperty(propertyName)
                .GetValue(this);
            return val == null ? "null" : val.ToString();
        }
        private enum SetMethod
        {
            NotSet,
            Config,
            EnvironmentalVariable
        }

    }
    
}