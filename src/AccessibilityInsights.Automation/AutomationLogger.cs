// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Desktop.Telemetry;
using System.Collections.Generic;
using System.Linq;

namespace AccessibilityInsights.Automation
{
    internal static class AutomationLogger
    {
        /// <summary>
        /// Constructor to initialize behavior of Logger
        /// </summary>
        static AutomationLogger()
        {
            // disable telemetry for now based on a decision on telemetry from Automation.
            // we decided not to send a telemetry explicitly from Automation(Including powershell since there is no explicit user consent).
            // later we may enable it again with an explicit user consent. 
            Logger.IsTelemetryAllowed = false;
        }

        private static string convertMapToString(IReadOnlyDictionary<string, string> propertyBag)
        {
            if (propertyBag != null)
            {
                return string.Join(";", propertyBag.Select(x => x.Key).ToArray());
            }
            return string.Empty;
        }

        /// <summary>
        /// Publishes an action to telemetry
        /// </summary>
        /// <param name="action">The action this function will log</param>
        /// <param name="propertyBag">The Properties we will log, we will extract just the values</param>
        public static void LogAction(TelemetryAction action, IReadOnlyDictionary<string, string> propertyBag = null)
        {
            try
            {
                Logger.PublishTelemetryEvent(action, TelemetryProperty.AutomationParametersSpecified, convertMapToString(propertyBag));
            }
            catch
            {
                // Telemetry failures should not block proper operation
            }
        }

        /// <summary>
        /// Adds context properties for future LogAction calls
        /// This should be the first telemetry-related code called from automation
        /// Preceding calls to LogAction will not send telemetry
        /// </summary>
        public static void DeclareSource(string teamName = null)
        {
            try
            {
                Logger.IsTelemetryAllowed = true;

                if (!string.IsNullOrEmpty(teamName))
                {
                    Logger.AddOrUpdateContextProperty(TelemetryProperty.TeamID, teamName);
                }

                Logger.AddOrUpdateContextProperty(TelemetryProperty.SessionType, "Automation");
                Logger.AddOrUpdateContextProperty(TelemetryProperty.Version, Core.Misc.Utility.GetAppVersion());
            }
            catch
            {
                // Telemetry failures should not block proper operation
            }
        }
    }
}
