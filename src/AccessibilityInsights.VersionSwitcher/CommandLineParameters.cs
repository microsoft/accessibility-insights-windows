// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AccessibilityInsights.VersionSwitcher
{
    internal class CommandLineParameters
    {
        internal string MsiPath { get; }

        internal string NewRing { get; }

        internal CommandLineParameters(string msiPath, string newRing = null)
        {
            MsiPath = msiPath;
            NewRing = newRing;
        }
    }
}
