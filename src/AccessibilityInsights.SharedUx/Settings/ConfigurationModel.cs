// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Desktop.UIAutomation;
using AccessibilityInsights.DesktopUI.Enums;
using AccessibilityInsights.Extensions.Interfaces.BugReporting;
using AccessibilityInsights.RuleSelection;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.FileBug;
using AccessibilityInsights.SharedUx.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AccessibilityInsights.SharedUx.Settings
{
    /// <summary>
    /// Configuration class
    /// it contains all configuration data for AccessibilityInsights
    /// </summary>
    public partial class ConfigurationModel : ConfigurationBase, ICloneable
    {
#pragma warning disable CA2227 // Collection properties should be read only. however since these properties will be serialized/deserialized via JSON, exempted.
        #region Setting options

        /// <summary>
        /// Zoom level to set for embedded web browser as a percentage
        /// the percentage is based on DPI, so it may start off at values other than 100%
        /// </summary>
        public int ZoomLevel { get; set; }

        /// <summary>
        /// The connection to automatically connect to when AccessibilityInsights starts up
        /// </summary>
        [JsonIgnore]
        public IConnectionInfo SavedConnection { get; set; }

        /// <summary>
        /// MRU connections
        /// </summary>
        [JsonIgnore]
        public IConnectionCache CachedConnections { get; set; }

        /// <summary>
        /// Serialized form of SavedConnection
        /// </summary>
        public string SerializedSavedConnection
        {
            get => SavedConnection?.ToConfigString();
            set => SavedConnection = BugReporter.CreateConnectionInfo(value);
        }

        /// <summary>
        /// Serialized form of CachedConnections
        /// </summary>
        public string SerializedCachedConnections
        {
            get => CachedConnections?.ToConfigString();
            set => CachedConnections = BugReporter.CreateConnectionCache(value);
        }

        /// <summary>
        /// Test Report Path
        /// </summary>
        public string TestReportPath { get; set; }

        /// <summary>
        /// Event Record Path
        /// </summary>
        public string EventRecordPath { get; set; }

        /// <summary>
        /// Test Config type (default, office, or custom)
        /// </summary>
        public SuiteConfigurationType TestConfig { get; set; }

        /// <summary>
        /// Path to test config path
        /// </summary>
        public string TestConfigPath { get; set; }

        /// <summary>
        /// Hot key for snap mode
        /// Shift + F7 is default value
        /// </summary>
        public string HotKeyForRecord { get; set; }

        /// <summary>
        /// Hot key for pause button
        /// Shift + F5 is default value
        /// </summary>
        public string HotKeyForPause { get; set; }

        /// <summary>
        /// Hot key for snap mode
        /// Shift + F8 is default value
        /// </summary>
        public string HotKeyForSnap { get; set; }

        /// <summary>
        /// Shift + F9
        /// Maximize and Minimize main window in live mode.
        /// </summary>
        public string HotKeyForActivatingMainWindow { get; set; }

        /// <summary>
        /// Hot key used for selecting an element using tree navigation.
        /// </summary>
        public string HotKeyForMoveToParent { get; set; }

        /// <summary>
        /// Hot key used for selecting an element using tree navigation.
        /// </summary>
        public string HotKeyForMoveToFirstChild { get; set; }

        /// <summary>
        /// Hot key used for selecting an element using tree navigation.
        /// </summary>
        public string HotKeyForMoveToLastChild { get; set; }

        /// <summary>
        /// Hot key used for selecting an element using tree navigation.
        /// </summary>
        public string HotKeyForMoveToNextSibbling { get; set; }

        /// <summary>
        /// Hot key used for selecting an element using tree navigation.
        /// </summary>
        public string HotKeyForMoveToPreviousSibbling { get; set; }

        /// <summary>
        /// Stores user's selected property selections
        /// </summary>
        public List<int> CoreProperties { get; set; }

        /// <summary>
        /// Stores user's selected text pattern attributes
        /// </summary>
        public List<int> CoreTPAttributes { get; set; }

        /// <summary>
        /// Mouse Selection Delay in MilliSeconds
        /// </summary>
        public int MouseSelectionDelayMilliSeconds { get; set; }

        /// <summary>
        /// Show splash window on launch
        /// </summary>
        public bool ShowWelcomeScreenOnLaunch { get; set; }

        /// <summary>
        /// If true, keyboard is used to select.
        /// </summary>
        public bool SelectionByFocus { get; set; }

        /// <summary>
        /// If true, Mouse is used to select.
        /// </summary>
        public bool SelectionByMouse { get; set; }

        /// <summary>
        /// If true, window will always be on top of other windows
        /// </summary>
        public bool AlwaysOnTop { get; set; }

        /// <summary>
        /// Indicate which font size to use
        /// </summary>
        public FontSize FontSize { get; set; } = FontSize.Standard;

        /// <summary>
        /// Indicate which hightlighter mode to use
        /// </summary>
        public HighlighterMode HighlighterMode {get; set;}

        /// <summary>
        /// If true, sound will be played while scanning is running with ATs
        /// </summary>
        public bool PlayScanningSound { get; set; }

        /// <summary>
        /// If true, tests will be run in snapshot mode
        /// </summary>
        public bool DisableTestsInSnapMode { get; set; }

        /// <summary>
        /// if it is true, show the full list of ancesters up to desktop in snap mode.
        /// </summary>
        public bool ShowAncestry { get; set; }

        /// <summary>
        /// if it is true, do not limit properties list to selected properties
        /// </summary>
        public bool ShowAllProperties { get; set; }

        /// <summary>
        /// Global state of highlighter
        /// </summary>
        [JsonIgnore]
        public bool IsHighlighterOn { get; set; } = true;

        /// <summary>
        /// Show/no show Uncertain results. 
        /// </summary>
        public bool ShowUncertain { get; set; }

        /// <summary>
        /// Replaces text pattern whitespace with formatting symbols
        /// </summary>
        public bool ShowWhitespaceInTextPatternViewer { get; set; }

        /// <summary>
        /// Tree view mode : Raw/Control/Content
        /// default is control
        /// </summary>
        public TreeViewMode TreeViewMode { get; set; }

        /// <summary>
        /// Inspect scope: Element/Entire App
        /// default is Element
        /// </summary>
        public bool IsUnderElementScope { get; set; }

        /// <summary>
        /// If true, telemetry will be collected
        /// </summary>
        public bool EnableTelemetry { get; set; }
        /// <summary>
        /// If true, telemetry startup dialog will be displayed
        /// </summary>
        public bool ShowTelemetryDialog { get; set; }
        #endregion
#pragma warning restore CA2227 // Collection properties should be read only

        /// <summary>
        /// Constructor
        /// </summary>
        public ConfigurationModel() { }

        /// <summary>
        /// Returns a copy of this object with relevant fields
        /// to send to telemetry
        /// </summary>
        /// <returns>new config model with stripped fields</returns>
        public ConfigurationModel CloneForTelemetry()
        {
            ConfigurationModel m = new ConfigurationModel();
            m.AlwaysOnTop = this.AlwaysOnTop;
            m.AppVersion = this.AppVersion;
            m.CoreProperties = this.CoreProperties;
            m.DisableTestsInSnapMode = this.DisableTestsInSnapMode;
            m.EventRecordPath = null; // scrubbed
            m.FontSize = this.FontSize;
            m.HotKeyForActivatingMainWindow = this.HotKeyForActivatingMainWindow;
            m.HotKeyForMoveToFirstChild = this.HotKeyForMoveToFirstChild;
            m.HotKeyForMoveToLastChild = this.HotKeyForMoveToLastChild;
            m.HotKeyForMoveToNextSibbling = this.HotKeyForMoveToNextSibbling;
            m.HotKeyForMoveToParent = this.HotKeyForMoveToParent;
            m.HotKeyForMoveToPreviousSibbling = this.HotKeyForMoveToPreviousSibbling;
            m.HotKeyForRecord = this.HotKeyForRecord;
            m.HotKeyForPause = this.HotKeyForPause;
            m.HotKeyForSnap = this.HotKeyForSnap;
            m.IsHighlighterOn = this.IsHighlighterOn;
            m.MouseSelectionDelayMilliSeconds = this.MouseSelectionDelayMilliSeconds;
            m.PlayScanningSound = this.PlayScanningSound;
            m.HighlighterMode = this.HighlighterMode;
            m.SelectionByFocus = this.SelectionByFocus;
            m.SelectionByMouse = this.SelectionByMouse;
            m.ShowAllProperties = this.ShowAllProperties;
            m.ShowAncestry = this.ShowAncestry;
            m.ShowUncertain = this.ShowUncertain;
            m.ShowWelcomeScreenOnLaunch = this.ShowWelcomeScreenOnLaunch;
            m.TestConfig = this.TestConfig;
            m.TestConfigPath = null; // scrubbed
            m.TestReportPath = null; // scrubbed
            m.TreeViewMode = this.TreeViewMode;
            m.Version = this.Version;
            m.ZoomLevel = this.ZoomLevel;
            m.IsUnderElementScope = this.IsUnderElementScope;
            m.EnableTelemetry = this.EnableTelemetry;
            m.ShowTelemetryDialog = this.ShowTelemetryDialog;
            return m;
        }

        /// <summary>
        /// Check whether we need to show Splash window or not
        /// </summary>
        /// <returns></returns>
        public bool NeedToShowWelcomeScreen()
        {
            if(ShowWelcomeScreenOnLaunch == false)
            {
                if(this.AppVersion != AccessibilityInsights.Core.Misc.Utility.GetAppVersion())
                {
                    this.AppVersion = AccessibilityInsights.Core.Misc.Utility.GetAppVersion();
                    this.ShowWelcomeScreenOnLaunch = true;
                    return true;
                }
            }

            return ShowWelcomeScreenOnLaunch; 
        }

        #region static method
        /// <summary>
        /// Get the current configuration model instance
        /// </summary>
        /// <returns></returns>
        public static ConfigurationModel LoadConfiguration(string path)
        {
            var config = ConfigurationModel.LoadFromJSON<ConfigurationModel>(path);

            if (config.Version != ConfigurationModel.CurrentVersion || ContainsNull(config))
            {
                // retain hot key mapping 
                string hksnapshot = config.HotKeyForSnap;
                string hkrecord = config.HotKeyForRecord == ConfigurationModel.OldHotKeyRecord ? null : config.HotKeyForRecord;
                string hkpause = config.HotKeyForPause;

                string hkactivate = config.HotKeyForActivatingMainWindow;

                config = GetDefaultConfigurationModel();

                config.HotKeyForRecord = hkrecord != null ? hkrecord : config.HotKeyForRecord;
                config.HotKeyForPause = hkpause != null ? hkpause : config.HotKeyForPause;
                config.HotKeyForSnap = hksnapshot;
                config.HotKeyForActivatingMainWindow = hkactivate;
            }

            if (config.CoreProperties.Count == 0)
            {
                config.CoreProperties = DesktopElementHelper.GetDefaultCoreProperties();
            }

            if (config.CoreTPAttributes == null)
            {
                config.CoreTPAttributes = new List<int>();
            }

            return config;
        }

        /// <summary>
        /// check if any hot key is null, like it was removed from the configuration file
        /// </summary>
        /// <param name="config"></param>
        private static bool ContainsNull(ConfigurationModel config)
        {
            return config.HotKeyForRecord == null
                || config.HotKeyForPause == null
                || config.HotKeyForSnap == null
                || config.HotKeyForActivatingMainWindow == null;
        }

        /// <summary>
        /// GetDefaultConfiguration Model
        /// </summary>
        /// <returns></returns>
        public static ConfigurationModel GetDefaultConfigurationModel()
        {          
            ConfigurationModel config = new ConfigurationModel();

            config.Version = ConfigurationModel.CurrentVersion;
            config.AppVersion = AccessibilityInsights.Core.Misc.Utility.GetAppVersion();

            config.TestReportPath = DirectoryManagement.sUserDataFolderPath;
            config.EventRecordPath = DirectoryManagement.sUserDataFolderPath;

            // select all available test suites
            config.HotKeyForRecord = ConfigurationModel.DefaultHotKeyRecord;
            config.HotKeyForPause = ConfigurationModel.DefaultHotKeyPause;
            config.HotKeyForSnap = ConfigurationModel.DefaultHotKeySnap;
            config.HotKeyForActivatingMainWindow = ConfigurationModel.DefaultHotKeyActivatingMainWindow;
            config.HotKeyForMoveToParent = ConfigurationModel.DefaultHotKeyMoveToParent;
            config.HotKeyForMoveToFirstChild = ConfigurationModel.DefaultHotKeyMoveToFirstChild;
            config.HotKeyForMoveToLastChild = ConfigurationModel.DefaultHotKeyMoveToLastChild;
            config.HotKeyForMoveToNextSibbling = ConfigurationModel.DefaultHotKeyMoveToNextSibbling;
            config.HotKeyForMoveToPreviousSibbling = ConfigurationModel.DefaultHotKeyMoveToPreviousSibbling;

            config.CoreProperties = DesktopElementHelper.GetDefaultCoreProperties();
            config.CoreTPAttributes = new List<int>();

            config.MouseSelectionDelayMilliSeconds = ConfigurationModel.DefaultSelectionDelayMilliSeconds;
            config.SelectionByFocus = true;
            config.SelectionByMouse = true;
            config.ShowWelcomeScreenOnLaunch = true;
            config.AlwaysOnTop = true;
            config.PlayScanningSound = true;  // turn it on by default
            config.DisableTestsInSnapMode = false; // Run test in snapshot
            config.IsHighlighterOn = true; // turn on highlighter
            config.ShowUncertain = false; // not show
            config.TreeViewMode = TreeViewMode.Control; // control mode as default
            config.FontSize = FontSize.Standard;
            config.HighlighterMode = HighlighterMode.HighlighterBeakerTooltip;
            config.ShowAncestry = true; // show ancestry by default.
            config.ZoomLevel = 100; // default zoom for file bug dialog is 100%
            config.EnableTelemetry = true; // telemetry is on by default
            config.ShowTelemetryDialog = true; // telemetry dialog is on by default

            config.TestConfig = SuiteConfigurationType.Default;
            config.IsUnderElementScope = true; // Element scope as default

            config.CachedConnections = BugReporter.CreateConnectionCache(config.SerializedCachedConnections);
            config.SavedConnection = BugReporter.CreateConnectionInfo(config.SerializedSavedConnection);

            return config;
        }

        /// <summary>
        /// Creates a dictionary of all the properties with different values between 2 models
        /// </summary>
        /// <param name="oldModel">the old model</param>
        /// <param name="newModel">the new model</param>
        /// <returns>A dictionary of all the properties that has a new value. Key is the propertyInfo and value is new property value</returns>
        public static IReadOnlyDictionary<PropertyInfo, object> Diff(ConfigurationModel oldModel, ConfigurationModel newModel)
        {
            Dictionary<PropertyInfo, object> diff = new Dictionary<PropertyInfo, object>();

            Type type = typeof(ConfigurationModel);
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                object oldValue = propertyInfo.GetValue(oldModel);
                object newValue = propertyInfo.GetValue(newModel);
                if (!object.Equals(oldValue, newValue))
                {
                    diff.Add(propertyInfo, propertyInfo.GetValue(newModel));
                }
            }
            return diff;
        }

        /// <summary>
        /// Clones the current configuration model - shallow clone
        /// </summary>
        /// <returns>A cloned configumation model</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
