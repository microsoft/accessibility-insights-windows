// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.Automation
{
    /// <summary>
    /// Class to start AccessibilityInsights (via StartCommand.Execute)
    /// </summary>
    public static class StartCommand
    {
        /// <summary>
        /// Execute the Start command. Used by both .NET and by PowerShell entry points
        /// </summary>
        /// <param name="primaryConfig">The primary calling parameters</param>
        /// <param name="configFile">Path to a config file containing the backup parameters</param>
        /// <returns>A StartCommandResult that describes the result of the command</returns>
        public static StartCommandResult Execute(Dictionary<string, string> primaryConfig, string configFile)
        {
            return Execute(primaryConfig, configFile, isPowerShell:false);
        }

        internal static StartCommandResult Execute(Dictionary<string, string> primaryConfig, string configFile, bool isPowerShell)
        {
            return ExecutionWrapper.ExecuteCommand(() =>
            {
                // Custom assembly resolver needs to be created before anything else, but only for PowerShell
                IDisposable customAssemblyResolver = isPowerShell ? new CustomAssemblyResolver() : null;
                CommandParameters parameters = new CommandParameters(primaryConfig, configFile);
                AutomationSession.NewInstance(parameters, customAssemblyResolver);
                return new StartCommandResult
                {
                    Completed = true,
                    SummaryMessage = DisplayStrings.SuccessStart,
                    Succeeded = true,
                };
            }, ErrorCommandResultFactory);
        }

        private static StartCommandResult ErrorCommandResultFactory(string errorDetail)
        {
            return new StartCommandResult
            {
                Completed = false,
                SummaryMessage = errorDetail,
                Succeeded = false,
            };
        }
    }
}
