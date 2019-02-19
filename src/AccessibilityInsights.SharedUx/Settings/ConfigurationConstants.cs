// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;

namespace AccessibilityInsights.SharedUx.Settings
{
    /// <summary>
    /// Configuration class
    /// it contains all configuration data for AccessibilityInsights
    /// </summary>
    public partial class ConfigurationModel : ConfigurationBase
    {
        /// <summary>
        /// Current version of configuration
        /// </summary>
        public const string CurrentVersion = "1.1.9";

        /// <summary>
        /// Default values for the config
        /// </summary>
        public const string DefaultTestReportsFolderName = "TestReports";
        public const string DefaultEventRecordsFolderName = "EventRecords";
        public const string DefaultHotKeyRecord = "Shift + F7";
        public const string DefaultHotKeyPause = "Shift + F5";
        public const string DefaultHotKeySnap = "Shift + F8";
        public const string OldHotKeyRecord = "Shift + F6";
        public const string DefaultHotKeyActivatingMainWindow = "Shift + F9";
        public const string DefaultHotKeyMoveToParent = "Control,Shift + F6";
        public const string DefaultHotKeyMoveToFirstChild = "Control,Shift + F7";
        public const string DefaultHotKeyMoveToLastChild = "Control,Shift + F9";
        public const string DefaultHotKeyMoveToNextSibbling = "Control,Shift + F8";
        public const string DefaultHotKeyMoveToPreviousSibbling = "Control,Shift + F5";
        public const string DefaultHotKeySave = "Control + S";
        public const int DefaultMouseSelectionDelayMilliSeconds = 800; // milliseconds 
        public const int DefaultSelectionDelayMilliSeconds = 100; // milliseconds 
        public const int MinimumSelectionDelayMilliSeconds = 50;
    }
}
