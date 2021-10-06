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
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                _systemShim.LogToSession("Caught Exception: " + e);
            }
#pragma warning restore CA1031 // Do not catch general exception types

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
                if (_systemShim.DirectoryExists(fileName)) 
                {
                    _systemShim.LogToSession("RemoveUserConfigFiles: Deleting directory: " + fileName);
                    _systemShim.DeleteDirectory(fileName);
                }
                else
                {
                    _systemShim.LogToSession("RemoveUserConfigFiles: Deleting file: " + fileName);
                    _systemShim.DeleteFile(fileName);
                }
            }
        }
    }
}