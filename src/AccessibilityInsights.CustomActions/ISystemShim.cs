// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace AccessibilityInsights.CustomActions
{
    /// <summary>
    /// Simple interface to allow unit testing of CustomActions.ConfigFileCleaner
    /// </summary>
    internal interface ISystemShim
    {
        IEnumerable<string> GetRunningProcessNames();
        void DeleteDirectory(string dirName);
        bool DirectoryExists(string dirName);
        void LogToSession(string message);
    }
}
