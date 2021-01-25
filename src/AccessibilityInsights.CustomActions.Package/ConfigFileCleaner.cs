// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Deployment.WindowsInstaller;
using System;

namespace AccessibilityInsights.CustomActions
{
    public class ConfigFileCleaner
    {
        private readonly ISystemShim _systemShim;

        [CustomAction]
        public static ActionResult RemoveUserConfigFiles(Session session)
        {
            ConfigFileCleaner cleaner = new ConfigFileCleaner(new SystemShim(session));
            return cleaner.RunAction();
        }

        internal ConfigFileCleaner(ISystemShim systemShim)
        {
            _systemShim = systemShim;
        }

        internal ActionResult RunAction()
        {
            try
            {
                if (IsVersionSwitcherRunning())
                {
                    _systemShim.LogToSession("RemoveUserConfigFiles: Found VersionSwitcher, ignoring config files.");
                }
                else
                {
                    DeleteConfigFiles();
                }
            }
            catch (Exception e)
            {
                _systemShim.LogToSession("Caught Exception: " + e);
            }

            return ActionResult.Success;  // Don't block installer if an error occurs
        }

        private bool IsVersionSwitcherRunning()
        {
            foreach (var processName in _systemShim.GetRunningProcessNames())
            {
                if (processName.Equals("AccessibilityInsights.VersionSwitcher", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private void DeleteConfigFiles()
        {
            _systemShim.LogToSession("RemoveUserConfigFiles: Finding config files");
            foreach (string fileName in _systemShim.GetConfigFiles())
            {
                _systemShim.LogToSession("RemoveUserConfigFiles: Deleting file: " + fileName);
                _systemShim.DeleteFile(fileName);
            }
        }
    }
}