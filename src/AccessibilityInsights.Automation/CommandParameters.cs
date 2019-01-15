// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Newtonsoft.Json;

namespace AccessibilityInsights.Automation
{
    /// <summary>
    /// Class to encapsulate the data inputs for commands
    /// </summary>
    internal class CommandParameters
    {
        private IReadOnlyDictionary<string, string> config;

        /// <summary>
        /// Whether or not a config file was used for input (we want this for telemetry)
        /// </summary>
        internal bool UsedConfigFile { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="primaryConfig">Code-based parameters. These "win" in case of conflict</param>
        /// <param name="configFile">File-based dictionary of parameters. These "lose" in case of conflict</param>
        internal CommandParameters(Dictionary<string, string> primaryConfig, string configFile)
        {
            Dictionary<string, string> tempConfig = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            UsedConfigFile = PopulateSecondaryConfig(tempConfig, configFile);
            MergeParameters(tempConfig, primaryConfig);

            this.config = tempConfig;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="primaryConfig">Code-based parameters. These "win" in case of conflict</param>
        /// <param name="sessionParameters">Session parameters. These "lose" in case of conflict</param>
        internal CommandParameters(Dictionary<string, string> primaryConfig, CommandParameters sessionParameters)
        {
            Dictionary<string, string> tempConfig = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            MergeParameters(tempConfig, sessionParameters.config);
            MergeParameters(tempConfig, primaryConfig);

            this.config = tempConfig;
        }

        /// <summary>
        /// Attempt to load a string value from the input parameters
        /// </summary>
        /// <param name="key">The key (case-insensitve) to use</param>
        /// <param name="value">Receives the parameter value</param>
        /// <returns>True iff the parameter was found</returns>
        internal bool TryGetString(string key, out string value)
        {
            return this.config.TryGetValue(key, out value);
        }

        /// <summary>
        /// Attempt to load a long value from the input parameters
        /// </summary>
        /// <param name="key">The key (case-insensitve) to use</param>
        /// <param name="value">Receives the parameter value</param>
        /// <returns>True iff the parameter was found</returns>
        internal bool TryGetLong(string key, out long value)
        {
            string stringValue;

            if (this.config.TryGetValue(key, out stringValue))
            {
                return long.TryParse(stringValue, out value);
            }

            value = default(long);
            return false;
        }

        /// <summary>
        /// Attempt to load a bool value from the input parameters
        /// </summary>
        /// <param name="key">The key (case-insensitve) to use</param>
        /// <param name="value">Receives the parameter value</param>
        /// <returns>True iff the parameter was found</returns>
        internal bool TryGetBool(string key, out bool value)
        {
            string stringValue;

            if (this.config.TryGetValue(key, out stringValue))
            {
                return bool.TryParse(stringValue, out value);
            }

            value = default(bool);
            return false;
        }

        /// <summary>
        /// Provide a copy of the configs in this object. Used for telemetry
        /// </summary>
        internal Dictionary<string, string> ConfigCopy
        {
            get
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                MergeParameters(result, this.config);
                return result;
            }
        }

        /// <summary>
        /// Merge parameters from source into config
        /// </summary>
        /// <param name="config">The config being built</param>
        /// <param name="source">The parameters to merge with override on key collision</param>
        private static void MergeParameters(Dictionary<string, string> config, IReadOnlyDictionary<string, string> source)
        {
            foreach (KeyValuePair<string, string> pair in source)
            {
                config[pair.Key] = pair.Value;
            }
        }

        /// <summary>
        /// Read the contents of the secondary config file into the config. We don't need to
        /// check for existence, since the primary contents will overwrite the secondary in case
        /// of conflict
        /// </summary>
        /// <param name="config">The Dictionary that holds the config</param>
        /// <param name="secondaryConfigFile">The path to the secondary config file. Can be empty</param>
        /// <returns>true if the secondaryConfigFile was used</returns>
        private static bool PopulateSecondaryConfig(Dictionary<string, string> config, string secondaryConfigFile)
        {
            if (string.IsNullOrWhiteSpace(secondaryConfigFile))
                return false;

            if (!File.Exists(secondaryConfigFile))
                throw new A11yAutomationException(string.Format(CultureInfo.InvariantCulture, DisplayStrings.ErrorCantFindSecondaryConfigFileFormat, secondaryConfigFile));

            string jsonData = File.ReadAllText(secondaryConfigFile);

            if (string.IsNullOrWhiteSpace(jsonData))
                throw new A11yAutomationException(string.Format(CultureInfo.InvariantCulture, DisplayStrings.ErrorEmptySecondaryConfigFile, secondaryConfigFile));

            try
            {
                foreach (KeyValuePair<string, string> pair in JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData))
                {
                    config[pair.Key] = pair.Value;
                }
                return true;
            }
            catch (Exception e)
            {
                throw new A11yAutomationException(string.Format(CultureInfo.InvariantCulture, DisplayStrings.ErrorInvalidSecondaryConfigFile, secondaryConfigFile), e);
            }
        }
    }
}
