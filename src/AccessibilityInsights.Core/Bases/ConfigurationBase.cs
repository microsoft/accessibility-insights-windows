// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Newtonsoft.Json;
using System;
using System.IO;

using static System.FormattableString;

namespace AccessibilityInsights.Core.Bases
{
    /// <summary>
    /// Base class for configuration files. Subclasses for individual configuration files 
    /// extend this class. Contains data and methods shared accross all configuration
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
        /// <param name="path"></param>
        public static void RemoveConfiguration(string path)
        {
            if (File.Exists(path))
            {
                File.Move(path, Invariant($"{path}.{DateTime.Now.Ticks}.bak"));
            }
        }

        /// <summary>
        /// Load config file from specified path
        /// </summary>
        /// <typeparam name="T">Configuration type</typeparam>
        /// <param name="kc">Calling config object</param>
        /// <param name="path">Config file location and name</param>
        /// <returns></returns>
        public static T LoadFromJSON<T>(string path)
        {
            T config = default(T);

            if (path != null && File.Exists(path))
            {
                var json = File.ReadAllText(path);
                config = JsonConvert.DeserializeObject<T>(json);
            }

            return config;
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
            catch
            {
                // ignore silently
            }

            return config;
        }
        # endregion
    }
}
