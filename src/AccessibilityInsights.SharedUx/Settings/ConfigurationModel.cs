// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SetupLibrary;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Misc;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using FileHelpers = AccessibilityInsights.SetupLibrary.FileHelpers;

namespace AccessibilityInsights.SharedUx.Settings
{
    /// <summary>
    /// Configuration class
    /// it contains all configuration data for AccessibilityInsights
    /// </summary>
    public partial class ConfigurationModel : ICloneable
    {
        /// <summary>
        /// The SettingsDictionary contains all of the persisted data
        /// </summary>
        private readonly SettingsDictionary _settings = new SettingsDictionary();

        #region Data Helpers

        /// <summary>
        /// Get a data value of the specific type--don't use for enumerated values
        /// </summary>
        /// <returns>The data if it exists, or the default if not</returns>
        private T GetDataValue<T>(string key)
        {
            if (_settings.TryGetValue(key, out object dataValue))
            {
                if (dataValue != null && (dataValue.GetType() == typeof(T)))
                {
                    return (T)dataValue;
                }
            }

            return default(T);
        }

        /// <summary>
        /// Set a data value of the specific type--don't use for enumerated values
        /// </summary>
        /// <remarks>The type 'T' is not strictly needed, but included for extra build-time checking</remarks>
        private void SetDataValue<T>(string key, T value)
        {
            _settings[key] = value;
        }

        /// <summary>
        /// Get an enumerated value (stored by name, not by number)
        /// </summary>
        /// <returns>The data if it exists, or the default if not</returns>
        private T GetEnumDataValue<T>(string key) where T : struct
        {
            if (_settings.TryGetValue(key, out object stringValue))
            {
                if (stringValue != null && (stringValue.GetType() == typeof(string)))
                {
                    if (Enum.TryParse<T>((string)stringValue, out T dataValue))
                    {
                        return dataValue;
                    }
                }
            }

            return default(T);
        }

        /// <summary>
        /// Get an enumerated value (stored by name, not by number), with a custom default
        /// </summary>
        /// <returns>The data if it exists, or the provided default if not</returns>
        private T GetEnumDataValueWithDefault<T>(string key, T defaultValue) where T : struct
        {
            if (_settings.TryGetValue(key, out object stringValue))
            {
                if (stringValue.GetType() == typeof(string))
                {
                    if (Enum.TryParse<T>((string)stringValue, out T dataValue))
                    {
                        return dataValue;
                    }
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Set an enumerated value (stored by name, not by number)
        /// </summary>
        private void SetEnumDataValue<T>(string key, T value) where T : struct
        {
            _settings[key] = value.ToString();
        }

        #endregion

        #region Setting options

        /// <summary>
        /// Schema version of the app that saved the data file -- do not discard data based on this value!
        /// </summary>
        public string Version
        {
            get => GetDataValue<string>(keyVersion);
            set => SetDataValue<string>(keyVersion, value);
        }

        /// <summary>
        /// Version of the app that saved the data file
        /// </summary>
        public string AppVersion
        {
            get => GetDataValue<string>(keyAppVersion);
            set => SetDataValue<string>(keyAppVersion, value);
        }

        public Guid SelectedIssueReporter
        {
            get
            {
                if (Guid.TryParse(GetDataValue<string>(keySelectedIssueReporter), out Guid result))
                {
                    return result;
                }
                return Guid.Empty;
            }
            set => SetDataValue<string>(keySelectedIssueReporter, value.ToString());
        }

        public string IssueReporterSerializedConfigs
        {
            get => GetDataValue<string>(keyIssueReporterSerializedConfigs);
            set => SetDataValue<string>(keyIssueReporterSerializedConfigs, value);
        }

        /// <summary>
        /// Test Report Path
        /// </summary>
        public string TestReportPath
        {
            get => GetDataValue<string>(keyTestReportPath);
            set => SetDataValue<string>(keyTestReportPath, value);
        }

        /// <summary>
        /// Event Record Path
        /// </summary>
        public string EventRecordPath
        {
            get => GetDataValue<string>(keyEventRecordPath);
            set => SetDataValue<string>(keyEventRecordPath, value);
        }

        /// <summary>
        /// Hot key for snap mode
        /// Shift + F7 is default value
        /// </summary>
        public string HotKeyForRecord
        {
            get => GetDataValue<string>(keyHotKeyForRecord);
            set => SetDataValue<string>(keyHotKeyForRecord, value);
        }

        /// <summary>
        /// Hot key for pause button
        /// Shift + F5 is default value
        /// </summary>
        public string HotKeyForPause
        {
            get => GetDataValue<string>(keyHotKeyForPause);
            set => SetDataValue<string>(keyHotKeyForPause, value);
        }

        /// <summary>
        /// Hot key for snap mode
        /// Shift + F8 is default value
        /// </summary>
        public string HotKeyForSnap
        {
            get => GetDataValue<string>(keyHotKeyForSnap);
            set => SetDataValue<string>(keyHotKeyForSnap, value);
        }

        /// <summary>
        /// Shift + F9
        /// Maximize and Minimize main window in live mode.
        /// </summary>
        public string HotKeyForActivatingMainWindow
        {
            get => GetDataValue<string>(keyHotKeyForActivatingMainWindow);
            set => SetDataValue<string>(keyHotKeyForActivatingMainWindow, value);
        }

        /// <summary>
        /// Hot key used for selecting an element using tree navigation.
        /// </summary>
        public string HotKeyForMoveToParent
        {
            get => GetDataValue<string>(keyHotKeyForMoveToParent);
            set => SetDataValue<string>(keyHotKeyForMoveToParent, value);
        }

        /// <summary>
        /// Hot key used for selecting an element using tree navigation.
        /// </summary>
        public string HotKeyForMoveToFirstChild
        {
            get => GetDataValue<string>(keyHotKeyForMoveToFirstChild);
            set => SetDataValue<string>(keyHotKeyForMoveToFirstChild, value);
        }

        /// <summary>
        /// Hot key used for selecting an element using tree navigation.
        /// </summary>
        public string HotKeyForMoveToLastChild
        {
            get => GetDataValue<string>(keyHotKeyForMoveToLastChild);
            set => SetDataValue<string>(keyHotKeyForMoveToLastChild, value);
        }

        /// <summary>
        /// Hot key used for selecting an element using tree navigation.
        /// </summary>
        public string HotKeyForMoveToNextSibling
        {
            get => GetDataValue<string>(keyHotKeyForMoveToNextSibling);
            set => SetDataValue<string>(keyHotKeyForMoveToNextSibling, value);
        }

        /// <summary>
        /// Hot key used for selecting an element using tree navigation.
        /// </summary>
        public string HotKeyForMoveToPreviousSibling
        {
            get => GetDataValue<string>(keyHotKeyForMoveToPreviousSibling);
            set => SetDataValue<string>(keyHotKeyForMoveToPreviousSibling, value);
        }

        /// <summary>
        /// Stores user's selected property selections
        /// </summary>
        public IEnumerable<int> CoreProperties
        {
            get => GetDataValue<int[]>(keyCoreProperties);
            set => SetDataValue<int[]>(keyCoreProperties, value.ToArray());
        }

        /// <summary>
        /// Stores user's selected text pattern attributes
        /// </summary>
        public IEnumerable<int> CoreTPAttributes
        {
            get => GetDataValue<int[]>(keyCoreTPAttributes);
            set => SetDataValue<int[]>(keyCoreTPAttributes, value.ToArray());
        }

        /// <summary>
        /// Mouse Selection Delay in MilliSeconds
        /// </summary>
        public int MouseSelectionDelayMilliSeconds
        {
            get => GetDataValue<int>(keyMouseSelectionDelayMilliSeconds);
            set => SetDataValue<int>(keyMouseSelectionDelayMilliSeconds, value);
        }

        /// <summary>
        /// Show splash window on launch
        /// </summary>
        public bool ShowWelcomeScreenOnLaunch
        {
            get => GetDataValue<bool>(keyShowWelcomeScreenOnLaunch);
            set => SetDataValue<bool>(keyShowWelcomeScreenOnLaunch, value);
        }

        /// <summary>
        /// If true, keyboard is used to select.
        /// </summary>
        public bool SelectionByFocus
        {
            get => GetDataValue<bool>(keySelectionByFocus);
            set => SetDataValue<bool>(keySelectionByFocus, value);
        }

        /// <summary>
        /// If true, Mouse is used to select.
        /// </summary>
        public bool SelectionByMouse
        {
            get => GetDataValue<bool>(keySelectionByMouse);
            set => SetDataValue<bool>(keySelectionByMouse, value);
        }

        /// <summary>
        /// If true, window will always be on top of other windows
        /// </summary>
        public bool AlwaysOnTop
        {
            get => GetDataValue<bool>(keyAlwaysOnTop);
            set => SetDataValue<bool>(keyAlwaysOnTop, value);
        }

        /// <summary>
        /// Indicate which font size to use
        /// </summary>
        public FontSize FontSize
        {
            get => GetEnumDataValueWithDefault<FontSize>(keyFontSize, FontSize.Standard);
            set => SetEnumDataValue<FontSize>(keyFontSize, value);
        }

        /// <summary>
        /// Indicate which hightlighter mode to use
        /// </summary>
        public HighlighterMode HighlighterMode
        {
            get => GetEnumDataValue<HighlighterMode>(keyHighlighterMode);
            set => SetEnumDataValue<HighlighterMode>(keyHighlighterMode, value);
        }

        /// <summary>
        /// If true, sound will be played while scanning is running with ATs
        /// </summary>
        public bool PlayScanningSound
        {
            get => GetDataValue<bool>(keyPlayScanningSound);
            set => SetDataValue<bool>(keyPlayScanningSound, value);
        }

        /// <summary>
        /// If true, tests will be run in snapshot mode
        /// </summary>
        public bool DisableTestsInSnapMode
        {
            get => GetDataValue<bool>(keyDisableTestsInSnapMode);
            set => SetDataValue<bool>(keyDisableTestsInSnapMode, value);
        }

        /// <summary>
        /// if it is true, show the full list of ancesters up to desktop in snap mode.
        /// </summary>
        public bool ShowAncestry
        {
            get => GetDataValue<bool>(keyShowAncestry);
            set => SetDataValue<bool>(keyShowAncestry, value);
        }

        /// <summary>
        /// if it is true, do not limit properties list to selected properties
        /// </summary>
        public bool ShowAllProperties
        {
            get => GetDataValue<bool>(keyShowAllProperties);
            set => SetDataValue<bool>(keyShowAllProperties, value);
        }

        /// <summary>
        /// Global state of highlighter
        /// </summary>
        public bool IsHighlighterOn { get; set; } = true;

        /// <summary>
        /// Show/no show Uncertain results.
        /// </summary>
        public bool ShowUncertain
        {
            get => GetDataValue<bool>(keyShowUncertain);
            set => SetDataValue<bool>(keyShowUncertain, value);
        }

        /// <summary>
        /// Replaces text pattern whitespace with formatting symbols
        /// </summary>
        public bool ShowWhitespaceInTextPatternViewer
        {
            get => GetDataValue<bool>(keyShowWhitespaceInTextPatternViewer);
            set => SetDataValue<bool>(keyShowWhitespaceInTextPatternViewer, value);
        }

        /// <summary>
        /// Tree view mode : Raw/Control/Content
        /// default is control
        /// </summary>
        public TreeViewMode TreeViewMode
        {
            get => GetEnumDataValue<TreeViewMode>(keyTreeViewMode);
            set => SetEnumDataValue<TreeViewMode>(keyTreeViewMode, value);
        }

        /// <summary>
        /// Inspect scope: Element/Entire App
        /// default is Element
        /// </summary>
        public bool IsUnderElementScope
        {
            get => GetDataValue<bool>(keyIsUnderElementScope);
            set => SetDataValue<bool>(keyIsUnderElementScope, value);
        }

        /// <summary>
        /// The release channel configured for this client
        /// </summary>
        public ReleaseChannel ReleaseChannel
        {
            get => GetEnumDataValue<ReleaseChannel>(keyReleaseChannel);
            set => SetEnumDataValue<ReleaseChannel>(keyReleaseChannel, value);
        }

        /// <summary>
        /// If true, telemetry will be collected
        /// </summary>
        public bool EnableTelemetry
        {
            get => GetDataValue<bool>(keyEnableTelemetry);
            set => SetDataValue<bool>(keyEnableTelemetry, value);
        }

        /// <summary>
        /// If true, dark mode will be disabled
        /// </summary>
        public bool DisableDarkMode
        {
            get => GetDataValue<bool>(keyDisableDarkMode);
            set => SetDataValue<bool>(keyDisableDarkMode, value);
        }

        /// <summary>
        /// If true, telemetry startup dialog will be displayed
        /// </summary>
        public bool ShowTelemetryDialog
        {
            get => GetDataValue<bool>(keyShowTelemetryDialog);
            set => SetDataValue<bool>(keyShowTelemetryDialog, value);
        }

        /// <summary>
        /// Check whether we need to show Welcome window or not
        /// </summary>
        /// <returns></returns>
        public bool NeedToShowWelcomeScreen()
        {
            if (ShowWelcomeScreenOnLaunch == false)
            {
                if (this.AppVersion !=VersionTools.GetAppVersion())
                {
                    this.AppVersion = VersionTools.GetAppVersion();
                    this.ShowWelcomeScreenOnLaunch = true;
                    return true;
                }
            }

            return ShowWelcomeScreenOnLaunch;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ConfigurationModel()
        {
            AppVersion = VersionTools.GetAppVersion();
            Version = CurrentVersion;
        }

        /// <summary>
        /// ctor from DiskConfigurationModel
        /// </summary>
        private ConfigurationModel(SettingsDictionary source)
        {
            _settings = new SettingsDictionary(source);
            AppVersion = VersionTools.GetAppVersion();
            Version = CurrentVersion;
        }

        /// <summary>
        /// Copy ctor
        /// </summary>
        /// <param name="source"></param>
        private ConfigurationModel(ConfigurationModel source)
        {
            _settings = new SettingsDictionary(source._settings);
        }

        #endregion

        #region File Management methods

        /// <summary>
        /// Save the current configuration model data to disk
        /// </summary>
        /// <param name="path">The path to the client's data file</param>
        public void SerializeInJSON(string path)
        {
            FileHelpers.SerializeDataToJSON(_settings, path);
        }

        #region static methods

        /// <summary>
        /// Rename the existing configuration to .bak file.
        /// </summary>
        /// <param name="path">The file to rename</param>
        public static void RemoveConfiguration(string path)
        {
            FileHelpers.RenameFileAsBackup(path);
        }

        /// <summary>
        /// Load the configuration model data from disk
        /// </summary>
        /// <param name="path">The path to the client's data file</param>
        /// <returns>The ConfigurationModel to use</returns>
        public static ConfigurationModel LoadFromJSON(string path, FixedConfigSettingsProvider provider)
        {
            ConfigurationModel config = LoadDataFromJSON(path) ?? GetDefaultConfigurationModel(provider);

            if (ContainsNull(config))
            {
                // retain hot key mapping
                string hksnapshot = config.HotKeyForSnap;
                string hkrecord = config.HotKeyForRecord == ConfigurationModel.OldHotKeyRecord ? null : config.HotKeyForRecord;
                string hkpause = config.HotKeyForPause;

                string hkactivate = config.HotKeyForActivatingMainWindow;

                config = GetDefaultConfigurationModel(provider);

                config.HotKeyForRecord = hkrecord != null ? hkrecord : config.HotKeyForRecord;
                config.HotKeyForPause = hkpause != null ? hkpause : config.HotKeyForPause;
                config.HotKeyForSnap = hksnapshot;
                config.HotKeyForActivatingMainWindow = hkactivate;
            }

            if (!config.CoreProperties.Any())
            {
                config.CoreProperties = PropertySettings.DefaultCoreProperties;
            }

            if (config.CoreTPAttributes == null)
            {
                config.CoreTPAttributes = new List<int>();
            }

            return config;
        }

        private static ConfigurationModel LoadDataFromJSON(string path)
        {
            var config = FileHelpers.LoadDataFromJSON<SettingsDictionary>(path);

            if (config != null && config.Any())
            {
                // Fix misspelled keys from legacy schema
                config.RemapSetting(keyHotKeyLegacyForMoveToNextSibling, keyHotKeyForMoveToNextSibling);
                config.RemapSetting(keyHotKeyLegacyForMoveToPreviousSibling, keyHotKeyForMoveToPreviousSibling);

                // Convert legacy values that are stored as numbers instead of enum names
                config.RemapIntToEnumName<TreeViewMode>(keyTreeViewMode);
                config.RemapIntToEnumName<HighlighterMode>(keyHighlighterMode);
                config.RemapIntToEnumName<FontSize>(keyFontSize);

                return new ConfigurationModel(config);
            }

            return null;
        }

        #endregion

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
        public static ConfigurationModel GetDefaultConfigurationModel(FixedConfigSettingsProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            ConfigurationModel config = new ConfigurationModel
            {
                AppVersion = VersionTools.GetAppVersion(),
                Version = CurrentVersion,

                TestReportPath = provider.UserDataFolderPath,
                EventRecordPath = provider.UserDataFolderPath,

                HotKeyForRecord = ConfigurationModel.DefaultHotKeyRecord,
                HotKeyForPause = ConfigurationModel.DefaultHotKeyPause,
                HotKeyForSnap = ConfigurationModel.DefaultHotKeySnap,
                HotKeyForActivatingMainWindow = ConfigurationModel.DefaultHotKeyActivatingMainWindow,
                HotKeyForMoveToParent = ConfigurationModel.DefaultHotKeyMoveToParent,
                HotKeyForMoveToFirstChild = ConfigurationModel.DefaultHotKeyMoveToFirstChild,
                HotKeyForMoveToLastChild = ConfigurationModel.DefaultHotKeyMoveToLastChild,
                HotKeyForMoveToNextSibling = ConfigurationModel.DefaultHotKeyMoveToNextSibling,
                HotKeyForMoveToPreviousSibling = ConfigurationModel.DefaultHotKeyMoveToPreviousSibling,

                CoreProperties = PropertySettings.DefaultCoreProperties,
                CoreTPAttributes = new List<int>(),

                MouseSelectionDelayMilliSeconds = ConfigurationModel.DefaultSelectionDelayMilliseconds,
                SelectionByFocus = true,
                SelectionByMouse = true,
                ShowWelcomeScreenOnLaunch = true,
                AlwaysOnTop = true,
                PlayScanningSound = false,
                DisableTestsInSnapMode = false,
                IsHighlighterOn = true,
                ShowUncertain = false,
                TreeViewMode = TreeViewMode.Control,
                FontSize = FontSize.Standard,
                HighlighterMode = HighlighterMode.HighlighterBeakerTooltip,
                ShowAncestry = true,
                EnableTelemetry = false,
                ShowTelemetryDialog = true,

                IsUnderElementScope = true,
                IssueReporterSerializedConfigs = null,
                SelectedIssueReporter = Guid.Empty,
            };

            return config;
        }

        /// <summary>
        /// Creates a dictionary of all the properties with different values between 2 models
        /// </summary>
        /// <param name="oldModel">the old model</param>
        /// <param name="newModel">the new model</param>
        /// <returns>A dictionary of all the properties that has a new value. Key is the name of the key and value is new property value</returns>
        public static IReadOnlyDictionary<string, object> Diff(ConfigurationModel oldModel, ConfigurationModel newModel)
        {
            if (oldModel == null)
                throw new ArgumentNullException(nameof(oldModel));
            if (newModel == null)
                throw new ArgumentNullException(nameof(newModel));

            return oldModel._settings.Diff(newModel._settings);
        }

        /// <summary>
        /// Clones the current configuration model - shallow clone
        /// </summary>
        /// <returns>A cloned ConfigurationModel</returns>
        public object Clone()
        {
            return new ConfigurationModel(this);
        }
        #endregion
    }
}
