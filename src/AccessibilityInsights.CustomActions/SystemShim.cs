// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Deployment.WindowsInstaller;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AccessibilityInsights.CustomActions
{
    /// <summary>
    /// Production implementation of ISystemShim
    /// </summary>
    internal class SystemShim : ISystemShim
    {
        private readonly Session _session;

        public SystemShim(Session session)
        {
            _session = session;
        }

        public IEnumerable<string> GetRunningProcessNames()
        {
            foreach (Process process in Process.GetProcesses())
            {
                yield return process.ProcessName;
            }
        }

        public void DeleteDirectory(string dirName)
        {
            Directory.Delete(dirName, true);
        }

        public bool DirectoryExists(string dirName)
        {
            return Directory.Exists(dirName);
        }

        public void LogToSession(string message)
        {
            _session.Log(message);
        }
    }
}
