// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using CommandLine;

namespace AccessibilityInsights.Misc
{
    /// <summary>
    /// command line options for Accessibility Insights for Windows
    ///
    /// with the exception of FileToOpen, all commands are intended for internal testing only
    /// </summary>
    public class CommandOptions
    {
        /// <summary>
        /// Saved test/events file that Accessibility Insights for Windows should open
        /// </summary>
        [Value(0, Required = false)]
        public string FileToOpen { get; set; }

        [Option('c', nameof(ConfigFolder), Required = false)]
        public string ConfigFolder { get; set; }

        [Option('u', nameof(UserDataFolder), Required = false)]
        public string UserDataFolder { get; set; }

        [Option('a', nameof(AttachToDebugger), Required = false)]
        public bool AttachToDebugger { get; set; }
    }
}
