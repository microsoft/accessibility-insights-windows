// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Misc;
using Axe.Windows.Telemetry;
using Newtonsoft.Json;
using System;

namespace AccessibilityInsights.SharedUx.Settings
{
    /// <summary>
    /// Base class for configuration files. Subclasses for individual configuration files
    /// extend this class. Contains data and methods shared across all configuration
    /// classes.
    /// </summary>
    public class ConfigurationBase
    {
        /// <summary>
        /// Version of this configuration
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Version of Application
        /// </summary>
        public string AppVersion { get; set; }

        /// <summary>
        /// Protected initial ctor--sets version to null
        /// </summary>
        /// <param name="version">The value for version (can be null)</param>
        protected ConfigurationBase() : this(null)
        {
        }

        /// <summary>
        /// Protected initial ctor--caller provides the version for the config object
        /// </summary>
        /// <param name="version">The value for version (can be null)</param>
        protected ConfigurationBase(string version)
        {
            Version = version;
            AppVersion = Misc.VersionTools.GetAppVersion();
        }

        /// <summary>
        /// Get the Json string of this instance
        /// </summary>
        /// <returns></returns>
        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        #region static methods
        /// <summary>
        /// Rename the existing configuration to .bak file.
        /// </summary>
        /// <param name="path">The file to rename</param>
        public static void RemoveConfiguration(string path)
        {
            FileHelpers.RenameFileAsBackup(path);
        }

        /// <summary>
        /// Load config file from specified path
        /// </summary>
        /// <typeparam name="T">Configuration type</typeparam>
        /// <param name="path">Config file location and name</param>
        /// <returns></returns>
        public static T LoadFromJSON<T>(string path)
        {
            return FileHelpers.LoadDataFromJSON<T>(path);
        }

        /// <summary>
        /// Store config file as JSON in given location
        /// </summary>
        /// <param name="path">Config file location and name</param>
        public void SerializeInJSON(string path)
        {
            FileHelpers.SerializeDataToJSON(this, path);
        }

        /// <summary>
        /// Load Configuration from String
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T FromString<T>(string text)
        {
            T config = default(T);

            try
            {
                config = JsonConvert.DeserializeObject<T>(text);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                e.ReportException();
                // ignore silently
            }
#pragma warning restore CA1031 // Do not catch general exception types

            return config;
        }
        # endregion
    }
}
