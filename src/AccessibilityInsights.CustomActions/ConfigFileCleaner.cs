// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SetupLibrary;
using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Diagnostics;
using System.IO;

namespace AccessibilityInsights.CustomActions
{
    public class ConfigFileCleaner
    {
        [CustomAction]
        public static ActionResult RemoveUserConfigFiles(Session session)
        {
            try
            {
                if (IsVersionSwitcherRunning())
                {
                    session.Log("RemoveUserConfigFiles: Found VersionSwitcher, leaving config files intact.");
                }
                else
                {
                    DeleteConfigFiles(session);
                }
            }
            catch (Exception e)
            {
                session.Log("Caught Exception: " + e);
            }

            return ActionResult.Success;  // Don't block installer if we can't clean up the files
        }

        private static bool IsVersionSwitcherRunning()
        {
            foreach (var x in Process.GetProcesses())
            {
                if (x.ProcessName.Equals("AccessibilityInsights.VersionSwitcher", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static void DeleteConfigFiles(Session session)
        {
            string configPath = FixedConfigSettingsProvider.CreateDefaultSettingsProvider().ConfigurationFolderPath;

            foreach (string fileName in Directory.EnumerateFiles(configPath))
            {
                session.Log("RemoveUserConfigFiles: Deleting file: {0}", fileName);
                File.Delete(fileName);
            }
        }
    }
}