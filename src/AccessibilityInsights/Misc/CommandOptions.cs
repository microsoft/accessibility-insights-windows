// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using CommandLine;

namespace AccessibilityInsights.Misc
{
    public class CommandOptions
    {
        [Value(0, HelpText = "File to open in Accessibility Insights", Required = false)]
        public string FileToOpen { get; set; }

        [Option('c', HelpText = "Path to app configuration folder", Required = false)]
        public string ConfigFolder { get; set; }

        [Option('u', HelpText = "Path to user data configuration folder", Required = false)]
        public string UserDataFolder { get; set; }

        [Option(HelpText = "Pause application startup to allow for attaching to debugger", Default = false)]
        public bool AttachToDebugger { get; set; }
    }
}
