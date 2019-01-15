// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Desktop.Telemetry;

namespace AccessibilityInsights.Automation
{
    /// <summary>
    /// Class to stop AccessibilityInsights (via StopCommand.Execute). Can only be successfully called after
    /// a successful call to StartCommand.Execute
    /// </summary>
    public static class StopCommand
    {
        /// <summary>
        /// Execute the Stop command. Used by both .NET and by PowerShell entry points
        /// </summary>
        /// <returns>A StopCommandResult that describes the result of the command</returns>
        public static StopCommandResult Execute()
        {
            return ExecutionWrapper.ExecuteCommand(() =>
            {
                AutomationLogger.LogAction(TelemetryAction.Automation_Stop_Session);
                AutomationSession.ClearInstance();
                return new StopCommandResult
                {
                    Completed = true,
                    SummaryMessage = DisplayStrings.SuccessStop,
                    Succeeded = true,
                };
            }, ErrorCommandResultFactory);
        }

        private static StopCommandResult ErrorCommandResultFactory(string errorDetail)
        {
            return new StopCommandResult
            {
                Completed = false,
                SummaryMessage = errorDetail,
                Succeeded = false,
            };
        }
    }
}
