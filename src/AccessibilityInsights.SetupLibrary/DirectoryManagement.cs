// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;

namespace AccessibilityInsights.SetupLibrary
{
    /// <summary>
    /// Encapsulates file paths used for user data and configuration
    /// Creates initial folders required to store AccessibilityInsights-related config files
    /// </summary>
    public static class DirectoryManagement
    {
        /// <summary>
        /// Create folder if it doesn't exist. 
        /// </summary>
        /// <param name="path"></param>
        public static void CreateFolder(string path)
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
