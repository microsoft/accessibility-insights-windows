// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccessibilityInsights.SetupLibrary
{
    /// <summary>
    /// Strongly typed dictionary that represents the config settings that persist to disk
    /// </summary>
    [Serializable]
#pragma warning disable CA2229 // Implement serialization constructors
    public class SettingsDictionary : SortedDictionary<string, object>
#pragma warning restore CA2229 // Implement serialization constructors
    {
        /// <summary>
        /// Typical constructor
        /// </summary>
        public SettingsDictionary() { }

        /// <summary>
        /// Copy constructor--converts longs to ints, JArrays to int arrays
        /// </summary>
        public SettingsDictionary(SettingsDictionary source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (KeyValuePair<string, object> pair in source)
            {
                if (pair.Value != null)
                {
                    Type type = pair.Value.GetType();

                    if (type == typeof(JArray))
                    {
                        var jArray = (JArray)(pair.Value);

                        List<int> list = new List<int>();
                        foreach (int value in jArray.Select(v => (int)v))
                        {
                            list.Add(value);
                        }
                        Add(pair.Key, list.ToArray());
                        continue;
                    }
                    if (type == typeof(long))
                    {
                        Add(pair.Key, (int)(long)pair.Value);
                        continue;
                    }
                }
                Add(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Return differences between two SettingsDictionary objects (this and other). The returned values will be from other
        /// </summary>
        /// <returns>The values from other that are different from this--values are considered
        /// different if they have incompatible types or different values</returns>
        public IReadOnlyDictionary<string, object> Diff(SettingsDictionary other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            Dictionary<string, object> diff = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> pair in this)
            {
                if (other.TryGetValue(pair.Key, out object otherValue))
                {
                    dynamic dynamicThisValue = pair.Value;
                    dynamic dynamicOtherValue = otherValue;

                    try
                    {
                        if (dynamicThisValue != dynamicOtherValue)
                        {
                            diff.Add(pair.Key, otherValue);
                        }
                    }
                    catch (RuntimeBinderException)
                    {
                        diff.Add(pair.Key, otherValue);
                    }
                }
            }

            return diff;
        }

        /// <summary>
        /// Remap an old setting's key to a new setting's key--used for legacy support
        /// </summary>
        /// <param name="oldSettingKey">The old setting key that will be deleted if it exists</param>
        /// <param name="newSettingKey">The new setting key that will replace the old setting key</param>
        /// <remarks>If both settings exist, the old setting will simply be discarded</remarks>
        public void RemapSetting(string oldSettingKey, string newSettingKey)
        {
            if (TryGetValue(oldSettingKey, out object value))
            {
                if (!TryGetValue(newSettingKey, out _))
                {
                    Add(newSettingKey, value);
                }

                Remove(oldSettingKey);
            }
        }

        /// <summary>
        /// Remap the value of a key from a number to its enum name. Used for legacy support
        /// </summary>
        /// <typeparam name="T">The type of the enum for the output data</typeparam>
        /// <param name="settingKey">The key of the setting to convert</param>
        public void RemapIntToEnumName<T>(string settingKey) where T : struct
        {
            if (TryGetValue(settingKey, out object value))
            {
                int? intValue = null;

                if (value.GetType() == typeof(long))
                {
                    intValue = (int)(long)value;
                }
                else if (value.GetType() == typeof(int))
                {
                    intValue = (int)value;
                }

                if (intValue.HasValue && typeof(T).IsEnum)
                {
                    List<int> enumValues = Enum.GetValues(typeof(T)).Cast<int>().ToList();

                    if (enumValues.Contains(intValue.Value))
                    {
                        this[settingKey] = ((T)Enum.ToObject(typeof(T), intValue.Value)).ToString();
                    }
                }
            }
        }
    }
}
