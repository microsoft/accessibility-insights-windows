// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AccessibilityInsights.SharedUx.Settings
{
    /// <summary>
    /// Configuration class
    /// it contains all configuration data for AccessibilityInsights
    /// </summary>
    public partial class ConfigurationModel
    {
        /// <summary>
        /// Current version of configuration (should change only when the schema changes, which should be rare)
        /// </summary>
        public const string CurrentVersion = "1.1.10";

        /// <summary>
        /// Default values for the config
        /// </summary>
        private const string DefaultHotKeyActivatingMainWindow = "Shift + F9";
        private const string DefaultHotKeyMoveToFirstChild = "Control,Shift + F7";
        private const string DefaultHotKeyMoveToLastChild = "Control,Shift + F9";
        private const string DefaultHotKeyMoveToNextSibling = "Control,Shift + F8";
        private const string DefaultHotKeyMoveToParent = "Control,Shift + F6";
        private const string DefaultHotKeyMoveToPreviousSibling = "Control,Shift + F5";
        private const string DefaultHotKeyPause = "Shift + F5";
        private const string DefaultHotKeyRecord = "Shift + F7";
        private const string DefaultHotKeySnap = "Shift + F8";
        private const string OldHotKeyRecord = "Shift + F6";
        private const int DefaultSelectionDelayMilliseconds = 100; // milliseconds
        internal const int MinimumSelectionDelayMilliseconds = 50;

        /// <summary>
        /// Strings to be used as key names in the dictionary--PLEASE RETAIN ALPHABETIC SORTING
        /// </summary>
        private const string keyAlwaysOnTop = "AlwaysOnTop";
        private const string keyAppVersion = "AppVersion";
        private const string keyCoreProperties = "CoreProperties";
        private const string keyCoreTPAttributes = "CoreTPAttributes";
        private const string keyDisableDarkMode = "DisableDarkMode";
        private const string keyDisableTestsInSnapMode = "DisableTestsInSnapMode";
        private const string keyEnableTelemetry = "EnableTelemetry";
        private const string keyEventRecordPath = "EventRecordPath";
        public const string keyFontSize = "FontSize";
        private const string keyHighlighterMode = "HighlighterMode";
        private const string keyHotKeyForActivatingMainWindow = "HotKeyForActivatingMainWindow";
        private const string keyHotKeyForMoveToFirstChild = "HotKeyForMoveToFirstChild";
        private const string keyHotKeyForMoveToLastChild = "HotKeyForMoveToLastChild";
        private const string keyHotKeyForMoveToNextSibling = "HotKeyForMoveToNextSibling";
        private const string keyHotKeyForMoveToParent = "HotKeyForMoveToParent";
        private const string keyHotKeyForMoveToPreviousSibling = "HotKeyForMoveToPreviousSibling";
        private const string keyHotKeyForPause = "HotKeyForPause";
        private const string keyHotKeyForRecord = "HotKeyForRecord";
        private const string keyHotKeyForSnap = "HotKeyForSnap";
        private const string keyHotKeyLegacyForMoveToNextSibling = "HotKeyForMoveToNextSibbling";
        private const string keyHotKeyLegacyForMoveToPreviousSibling = "HotKeyForMoveToPreviousSibbling";
        private const string keyIssueReporterSerializedConfigs = "IssueReporterSerializedConfigs";
        private const string keyIsUnderElementScope = "IsUnderElementScope";
        private const string keyMouseSelectionDelayMilliSeconds = "MouseSelectionDelayMilliSeconds";
        private const string keyPlayScanningSound = "PlayScanningSound";
        private const string keyReleaseChannel = SetupLibrary.Constants.ReleaseChannelKey;
        private const string keySelectedIssueReporter = "SelectedIssueReporter";
        private const string keySelectionByFocus = "SelectionByFocus";
        private const string keySelectionByMouse = "SelectionByMouse";
        private const string keyShowAllProperties = "ShowAllProperties";
        private const string keyShowAncestry = "ShowAncestry";
        private const string keyShowTelemetryDialog = "ShowTelemetryDialog";
        private const string keyShowUncertain = "ShowUncertain";
        private const string keyShowWelcomeScreenOnLaunch = "ShowWelcomeScreenOnLaunch";
        private const string keyShowWhitespaceInTextPatternViewer = "ShowWhitespaceInTextPatternViewer";
        private const string keyTestReportPath = "TestReportPath";
        private const string keyTreeViewMode = "TreeViewMode";
        private const string keyVersion = "Version";
    }
}
