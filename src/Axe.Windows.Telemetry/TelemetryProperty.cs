// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Axe.Windows.Telemetry
{
    /// <summary>
    /// Enums for Telemetry Properties
    /// </summary>
    public enum TelemetryProperty
    {
        Version, 
        ModeName, 
        ModeSessionId,
        FeedbackLevel,
        Feedback, // contains the actual Feedback Text
        ExceptionType,
        UnhandledException,
        HandledException,
        View,
        UIFramework,
        Boolean,
        MSIVersion,
        By,
        InteractionAllowed,
        Comment,
        TextPatternFrom,
        TextRangeType,
        IsAlreadyLoggedIn,
        TextRangeUnit,
        PromptIfNeeded,
        FileMode,
        ProtocolMode,
        Error,
        TreeWalkMode,
        Scope, // to indicate the scope of selection
        TabStopLooped,
        TabStopCount,
        Seconds,
        ControlType,
        RuleId,
        TestResults, // parent container, has rule id and results
        Results,
        SessionType,
        AppSessionID,
        PatternMethod,
        FilesLoaded,
        UpdatesMade,
        InstallationID,
        ElementsInScan,
        UpperBoundExceeded,
        TeamID,
        UpdateTimedOut,
        UpdateInitializationTime,
        UpdateOptionWaitTime,
        UpdateOption,
        UpdateInstallerDownloadTime,
        UpdateInstallerVerificationTime,
        UpdateResult,
    }
}
