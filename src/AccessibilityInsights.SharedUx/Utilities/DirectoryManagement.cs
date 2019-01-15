// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;

namespace AccessibilityInsights.SharedUx.Utilities
{
    /// <summary>
    /// Encapsulates file paths used for user data & configuration
    /// Creates initial folders required to store AccessibilityInsights-related config files
    /// </summary>
    public static class DirectoryManagement
    {
        /// <summary>
        /// Create necessary folders for Configurations and User data
        /// </summary>
        public static void CreateFolders()
        {
            CreateFolder(sUserDataFolderPath);
            CreateFolder(sConfigurationFolderPath);
        }

        /// <summary>
        /// Create folder if it doesn't exist. 
        /// </summary>
        /// <param name="path"></param>
        private static void CreateFolder(string path)
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Data storage folder
        /// </summary>
        const string UserDataPath = "AccessibilityInsights";

        /// <summary>
        /// Configuration folder path
        /// </summary>
        const string ConfigurationPath = @"AccessibilityInsights\V1\Configurations";

        /// <summary>
        /// Full path for user data storage folder
        /// </summary>
        public static readonly string sUserDataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments), UserDataPath);

        /// <summary>
        /// Full path for configuration foler
        /// it will be under LocalApplicationData folder
        /// </summary>
        public static readonly string sConfigurationFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ConfigurationPath);
    }
}
