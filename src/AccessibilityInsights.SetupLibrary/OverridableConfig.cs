// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AccessibilityInsights.SetupLibrary
{
    /// <summary>
    /// Class to provide an option to override config values via an external JSON file. This class
    /// does not respond to changes made to the file after the object is created
    /// </summary>
    class OverridableConfig
    {
        private readonly IReadOnlyDictionary<string, string> _settings;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="configFile">The name of the file that contains the config data</param>
        /// <param name="exceptionReporter">The optional sink for exceptions</param>
        internal OverridableConfig(string configFile, IExceptionReporter exceptionReporter)
        {
            string fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), configFile);
            if (File.Exists(fullPath))
            {
                try
                {
                    string json = File.ReadAllText(fullPath);
                    if (json.Length > 0)
                    {
                        _settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                        return;
                    }
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
                {
                    exceptionReporter?.ReportException(e);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }

            // This is our typical path
            _settings = new Dictionary<string, string>();
        }

        /// <summary>
        /// Return the config setting, based on the config file and the default value
        /// </summary>
        /// <param name="settingName">The name of the setting to setting</param>
        /// <param name="defaultValue">The default value for the setting</param>
        /// <returns>The value from the config file if it exists, the default value if not</returns>
        internal string GetConfigSetting(string settingName, string defaultValue)
        {
            return _settings.TryGetValue(settingName, out string result) ? result : defaultValue;
        }
    }
}
