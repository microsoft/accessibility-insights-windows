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
        private const string KeyAlwaysOnTop = "AlwaysOnTop";
        private const string KeyAppVersion = "AppVersion";
        private const string KeyCoreProperties = "CoreProperties";
        private const string KeyCoreTPAttributes = "CoreTPAttributes";
        private const string KeyDisableTestsInSnapMode = "DisableTestsInSnapMode";
        private const string KeyEnableTelemetry = "EnableTelemetry";
        private const string KeyEventRecordPath = "EventRecordPath";
        public const string KeyFontSize = "FontSize";
        private const string KeyHighlighterMode = "HighlighterMode";
        private const string KeyHotKeyForActivatingMainWindow = "HotKeyForActivatingMainWindow";
        private const string KeyHotKeyForMoveToFirstChild = "HotKeyForMoveToFirstChild";
        private const string KeyHotKeyForMoveToLastChild = "HotKeyForMoveToLastChild";
        private const string KeyHotKeyForMoveToNextSibling = "HotKeyForMoveToNextSibling";
        private const string KeyHotKeyForMoveToParent = "HotKeyForMoveToParent";
        private const string KeyHotKeyForMoveToPreviousSibling = "HotKeyForMoveToPreviousSibling";
        private const string KeyHotKeyForPause = "HotKeyForPause";
        private const string KeyHotKeyForRecord = "HotKeyForRecord";
        private const string KeyHotKeyForSnap = "HotKeyForSnap";
        private const string KeyHotKeyLegacyForMoveToNextSibling = "HotKeyForMoveToNextSibbling";
        private const string KeyHotKeyLegacyForMoveToPreviousSibling = "HotKeyForMoveToPreviousSibbling";
        private const string KeyIssueReporterSerializedConfigs = "IssueReporterSerializedConfigs";
        private const string KeyIsUnderElementScope = "IsUnderElementScope";
        private const string KeyLegacySoundFeedback = "PlayScanningSound";
        private const string KeyMouseSelectionDelayMilliSeconds = "MouseSelectionDelayMilliSeconds";
        private const string KeyReleaseChannel = SetupLibrary.Constants.ReleaseChannelKey;
        private const string KeySelectedIssueReporter = "SelectedIssueReporter";
        private const string KeySelectionByFocus = "SelectionByFocus";
        private const string KeySelectionByMouse = "SelectionByMouse";
        private const string KeyShowAllProperties = "ShowAllProperties";
        private const string KeyShowAncestry = "ShowAncestry";
        private const string KeyShowTelemetryDialog = "ShowTelemetryDialog";
        private const string KeyShowUncertain = "ShowUncertain";
        private const string KeyShowWelcomeScreenOnLaunch = "ShowWelcomeScreenOnLaunch";
        private const string KeyShowWhitespaceInTextPatternViewer = "ShowWhitespaceInTextPatternViewer";
        private const string KeySoundFeedback = "SoundFeedback";
        private const string KeyTestReportPath = "TestReportPath";
        private const string KeyTreeViewMode = "TreeViewMode";
        private const string KeyVersion = "Version";
    }
}
