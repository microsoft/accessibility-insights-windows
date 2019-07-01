// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;

namespace AccessibilityInsights.SetupLibrary
{
    public class FixedConfigSettingsProvider
    {
        public string ConfigurationFolderPath { get; }
        public string UserDataFolderPath { get; }

        public FixedConfigSettingsProvider(string configurationFolderPath, string userDataFolderPath)
        {
            this.ConfigurationFolderPath = configurationFolderPath;
            this.UserDataFolderPath = userDataFolderPath;
        }

        public static FixedConfigSettingsProvider CreateDefaultSettingsProvider()
        {
            var userDataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments), "AccessibilityInsights");
            var configurationFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"AccessibilityInsights\V1\Configurations");
            return new FixedConfigSettingsProvider(configurationFolderPath, userDataFolderPath);
        }
    }
}
