// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AccessibilityInsights.SharedUx.Telemetry
{
    /// <summary>
    /// Enums for Telemetry Properties
    /// </summary>
    public enum TelemetryProperty
    {
        Version,
        ModeName,
        ModeSessionId,
        View,
        UIFramework,
        MSIVersion,
        By,
        Comment,
        IsAlreadyLoggedIn,
        FileMode,
        Error,
        Scope, // to indicate the scope of selection
        TabStopLooped,
        TabStopCount,
        Seconds,
        RuleId,
        SessionType,
        AppSessionID,
        PatternMethod,
        InstallationID,
        UIAccessEnabled,
        UpdateTimedOut,
        UpdateInitializationTime,
        UpdateManifestRequestUri,
        UpdateManifestResponseUri,
        UpdateManifestSizeInBytes,
        UpdateOptionWaitTime,
        UpdateOption,
        UpdateInstallerUpdateTime,
        UpdateResult,
        ReleaseChannel,
        ReleaseChannelConsidered,
        IssueReporter,
        InstalledDotNetFrameworkVersion,
        OsArchitecture,
        CustomUIAPropertyCount,
        IsNowEnabled,
        Confidence, // For automatic colour contrast detection
        BitmapSize, // For automatic colour contrast detection

        // properties added for VersionSwitcher_VersionSwitcherResult
        StartingVersion,
        Result,
        RequestedMsi,
        ResolvedMsi,
        ExpectedMsiSize,
        ExpectedMsiSha512,
        ActualMsiSize,
        ActualMsiSha512,
        NewChannel,
        ExecutionTimeInMilliseconds,
    }
}
