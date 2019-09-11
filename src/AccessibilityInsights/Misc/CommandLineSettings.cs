// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using CommandLine;
using System;
using System.Linq;

namespace AccessibilityInsights.Misc
{
    /// <summary>
    /// Encapsulates user-defined settings passed in via the command line
    /// </summary>
    public static class CommandLineSettings
    {
        private static readonly Lazy<CommandOptions> Options = new Lazy<CommandOptions>(() => InitializeCommandOptions());

        private static CommandOptions InitializeCommandOptions()
        {
            var args = Environment.GetCommandLineArgs();
            using (var parser = new Parser(with => with.EnableDashDash = true))
            {
                return parser.ParseArguments<CommandOptions>(args.Skip(1))
                    .MapResult(parsed => parsed, options => new CommandOptions());
            }
        }

        public static string ConfigFolder => Options.Value.ConfigFolder;

        public static string UserDataFolder => Options.Value.UserDataFolder;

        public static bool AttachToDebugger => Options.Value.AttachToDebugger;

        public static string FileToOpen => Options.Value.FileToOpen;
    }
}
