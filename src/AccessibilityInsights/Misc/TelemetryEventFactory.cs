// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights.SetupLibrary;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms.Design;

namespace AccessibilityInsights.Misc
{
    /// <summary>
    /// Factory to create TelemetryEvents. To be used where TelemetryActions have properties, but the
    /// underlying types aren't too specialized
    /// </summary>
    internal static class TelemetryEventFactory
    {
        private static string GetTimeSpanTelemetryString(TimeSpan? timeSpan)
        {
            // Because return values are used for telemetry, they do not need to be localized
            return timeSpan.HasValue ? timeSpan.Value.ToString() : "unknown";
        }

        public static TelemetryEvent ForReleaseChannelChangeConsidered(ReleaseChannel oldChannel, ReleaseChannel newChannel)
        {
            return new TelemetryEvent(TelemetryAction.ReleaseChannel_ChangeConsidered,
                new Dictionary<TelemetryProperty, string>
                {
                    { TelemetryProperty.ReleaseChannel, oldChannel.ToString() },
                    { TelemetryProperty.ReleaseChannelConsidered, newChannel.ToString() },
                });
        }

        public static TelemetryEvent ForTestRequested(string method, string scope)
        {
            return new TelemetryEvent(TelemetryAction.Test_Requested,
                new Dictionary<TelemetryProperty, string>
                {
                    { TelemetryProperty.By, method },
                    { TelemetryProperty.Scope, scope },
                });
        }

        public static TelemetryEvent ForUpgradeDismissed(Version installedVersion)
        {
            return new TelemetryEvent(TelemetryAction.Upgrade_Update_Dismiss,
                new Dictionary<TelemetryProperty, string>
                {
                    { TelemetryProperty.MSIVersion, installedVersion.ToString() },
                });
        }

        public static TelemetryEvent ForUpgradeInstallationError(string error)
        {
            return new TelemetryEvent(TelemetryAction.Upgrade_InstallationError,
                new Dictionary<TelemetryProperty, string>
                {
                    { TelemetryProperty.Error, error },
                });
        }

        public static TelemetryEvent ForUpgradeDoInstallation(TimeSpan? duration, string updateResult)
        {
            return new TelemetryEvent(TelemetryAction.Upgrade_DoInstallation,
                new Dictionary<TelemetryProperty, string>
                {
                    { TelemetryProperty.UpdateInstallerUpdateTime, GetTimeSpanTelemetryString(duration) },
                    { TelemetryProperty.UpdateResult, updateResult },
                });
        }

        public static TelemetryEvent ForGetUpgradeOption(TimeSpan? initializationTime, TimeSpan? optionWaitTime, string updateOption, bool timedOut)
        {
            return new TelemetryEvent(TelemetryAction.Upgrade_GetUpgradeOption,
                new Dictionary<TelemetryProperty, string>
                {
                    { TelemetryProperty.UpdateInitializationTime, GetTimeSpanTelemetryString(initializationTime) },
                    { TelemetryProperty.UpdateOptionWaitTime, GetTimeSpanTelemetryString(optionWaitTime) },
                    { TelemetryProperty.UpdateOption, updateOption },
                    { TelemetryProperty.UpdateTimedOut, timedOut.ToString(CultureInfo.InvariantCulture) },
                });
        }

        public static TelemetryEvent ForSetScope(string scope)
        {
            return new TelemetryEvent(TelemetryAction.TestSelection_Set_Scope,
                new Dictionary<TelemetryProperty, string>
                {
                    { TelemetryProperty.Scope, scope },
                });
        }

        public static TelemetryEvent ForLoadDataFile(string mode)
        {
            return new TelemetryEvent(TelemetryAction.Hierarchy_Load_NewFormat,
                new Dictionary<TelemetryProperty, string>
                {
                    { TelemetryProperty.FileMode, mode },
                });
        }

        public static TelemetryEvent ForMainWindowStartup()
        {
            int? rawDotNetFrameworkVersion = NativeMethods.GetInstalledDotNetFrameworkVersion();
            string formattedDotNetFrameworkVersion = rawDotNetFrameworkVersion.HasValue ?
                rawDotNetFrameworkVersion.Value.ToString(CultureInfo.InvariantCulture) :
                "unknown";

            return new TelemetryEvent(TelemetryAction.Mainwindow_Startup,
                new Dictionary<TelemetryProperty, string>
                {
                    { TelemetryProperty.UIAccessEnabled, NativeMethods.IsRunningWithUIAccess().ToString(CultureInfo.InvariantCulture) },
                    { TelemetryProperty.InstalledDotNetFrameworkVersion, formattedDotNetFrameworkVersion }
                });
        }

        public static TelemetryEvent ForCustomUIAPropertyCount(int count)
        {
            return new TelemetryEvent(TelemetryAction.Custom_UIA,
                new Dictionary<TelemetryProperty, string>
                {
                    [TelemetryProperty.CustomUIAPropertyCount] = count.ToString(CultureInfo.InvariantCulture)
                }
            );
        }
    }
}
