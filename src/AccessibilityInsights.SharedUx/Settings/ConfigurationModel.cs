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
            get => GetDataValue<string>(KeyVersion);
            set => SetDataValue<string>(KeyVersion, value);
        }

        /// <summary>
        /// Version of the app that saved the data file
        /// </summary>
        public string AppVersion
        {
            get => GetDataValue<string>(KeyAppVersion);
            set => SetDataValue<string>(KeyAppVersion, value);
        }

        public Guid SelectedIssueReporter
        {
            get
            {
                if (Guid.TryParse(GetDataValue<string>(KeySelectedIssueReporter), out Guid result))
                {
                    return result;
                }
                return Guid.Empty;
            }
            set => SetDataValue<string>(KeySelectedIssueReporter, value.ToString());
        }

        public string IssueReporterSerializedConfigs
        {
            get => GetDataValue<string>(KeyIssueReporterSerializedConfigs);
            set => SetDataValue<string>(KeyIssueReporterSerializedConfigs, value);
        }

        /// <summary>
        /// Test Report Path
        /// </summary>
        public string TestReportPath
        {
            get => GetDataValue<string>(KeyTestReportPath);
            set => SetDataValue<string>(KeyTestReportPath, value);
        }

        /// <summary>
        /// Event Record Path
        /// </summary>
        public string EventRecordPath
        {
            get => GetDataValue<string>(KeyEventRecordPath);
            set => SetDataValue<string>(KeyEventRecordPath, value);
        }

        /// <summary>
        /// Hot key for snap mode
        /// Shift + F7 is default value
        /// </summary>
        public string HotKeyForRecord
        {
            get => GetDataValue<string>(KeyHotKeyForRecord);
            set => SetDataValue<string>(KeyHotKeyForRecord, value);
        }

        /// <summary>
        /// Hot key for pause button
        /// Shift + F5 is default value
        /// </summary>
        public string HotKeyForPause
        {
            get => GetDataValue<string>(KeyHotKeyForPause);
            set => SetDataValue<string>(KeyHotKeyForPause, value);
        }

        /// <summary>
        /// Hot key for snap mode
        /// Shift + F8 is default value
        /// </summary>
        public string HotKeyForSnap
        {
            get => GetDataValue<string>(KeyHotKeyForSnap);
            set => SetDataValue<string>(KeyHotKeyForSnap, value);
        }

        /// <summary>
        /// Shift + F9
        /// Maximize and Minimize main window in live mode.
        /// </summary>
        public string HotKeyForActivatingMainWindow
        {
            get => GetDataValue<string>(KeyHotKeyForActivatingMainWindow);
            set => SetDataValue<string>(KeyHotKeyForActivatingMainWindow, value);
        }

        /// <summary>
        /// Hot key used for selecting an element using tree navigation.
        /// </summary>
        public string HotKeyForMoveToParent
        {
            get => GetDataValue<string>(KeyHotKeyForMoveToParent);
            set => SetDataValue<string>(KeyHotKeyForMoveToParent, value);
        }

        /// <summary>
        /// Hot key used for selecting an element using tree navigation.
        /// </summary>
        public string HotKeyForMoveToFirstChild
        {
            get => GetDataValue<string>(KeyHotKeyForMoveToFirstChild);
            set => SetDataValue<string>(KeyHotKeyForMoveToFirstChild, value);
        }

        /// <summary>
        /// Hot key used for selecting an element using tree navigation.
        /// </summary>
        public string HotKeyForMoveToLastChild
        {
            get => GetDataValue<string>(KeyHotKeyForMoveToLastChild);
            set => SetDataValue<string>(KeyHotKeyForMoveToLastChild, value);
        }

        /// <summary>
        /// Hot key used for selecting an element using tree navigation.
        /// </summary>
        public string HotKeyForMoveToNextSibling
        {
            get => GetDataValue<string>(KeyHotKeyForMoveToNextSibling);
            set => SetDataValue<string>(KeyHotKeyForMoveToNextSibling, value);
        }

        /// <summary>
        /// Hot key used for selecting an element using tree navigation.
        /// </summary>
        public string HotKeyForMoveToPreviousSibling
        {
            get => GetDataValue<string>(KeyHotKeyForMoveToPreviousSibling);
            set => SetDataValue<string>(KeyHotKeyForMoveToPreviousSibling, value);
        }

        /// <summary>
        /// Stores user's selected property selections
        /// </summary>
        public IEnumerable<int> CoreProperties
        {
            get => GetDataValue<int[]>(KeyCoreProperties);
            set => SetDataValue<int[]>(KeyCoreProperties, value.ToArray());
        }

        /// <summary>
        /// Stores user's selected text pattern attributes
        /// </summary>
        public IEnumerable<int> CoreTPAttributes
        {
            get => GetDataValue<int[]>(KeyCoreTPAttributes);
            set => SetDataValue<int[]>(KeyCoreTPAttributes, value.ToArray());
        }

        /// <summary>
        /// Mouse Selection Delay in MilliSeconds
        /// </summary>
        public int MouseSelectionDelayMilliSeconds
        {
            get => GetDataValue<int>(KeyMouseSelectionDelayMilliSeconds);
            set => SetDataValue<int>(KeyMouseSelectionDelayMilliSeconds, value);
        }

        /// <summary>
        /// Show splash window on launch
        /// </summary>
        public bool ShowWelcomeScreenOnLaunch
        {
            get => GetDataValue<bool>(KeyShowWelcomeScreenOnLaunch);
            set => SetDataValue<bool>(KeyShowWelcomeScreenOnLaunch, value);
        }

        /// <summary>
        /// If true, keyboard is used to select.
        /// </summary>
        public bool SelectionByFocus
        {
            get => GetDataValue<bool>(KeySelectionByFocus);
            set => SetDataValue<bool>(KeySelectionByFocus, value);
        }

        /// <summary>
        /// If true, Mouse is used to select.
        /// </summary>
        public bool SelectionByMouse
        {
            get => GetDataValue<bool>(KeySelectionByMouse);
            set => SetDataValue<bool>(KeySelectionByMouse, value);
        }

        /// <summary>
        /// If true, window will always be on top of other windows
        /// </summary>
        public bool AlwaysOnTop
        {
            get => GetDataValue<bool>(KeyAlwaysOnTop);
            set => SetDataValue<bool>(KeyAlwaysOnTop, value);
        }

        /// <summary>
        /// Indicate which font size to use
        /// </summary>
        public FontSize FontSize
        {
            get => GetEnumDataValueWithDefault<FontSize>(KeyFontSize, FontSize.Standard);
            set => SetEnumDataValue<FontSize>(KeyFontSize, value);
        }

        /// <summary>
        /// Indicate which highlighter mode to use
        /// </summary>
        public HighlighterMode HighlighterMode
        {
            get => GetEnumDataValue<HighlighterMode>(KeyHighlighterMode);
            set => SetEnumDataValue<HighlighterMode>(KeyHighlighterMode, value);
        }

        /// <summary>
        /// Whether to provide feedback with sound: Auto (if screen reader flag is set), Always, or Never.
        /// </summary>
        public SoundFeedbackMode SoundFeedback
        {
            get => GetEnumDataValue<SoundFeedbackMode>(KeySoundFeedback);
            set => SetEnumDataValue<SoundFeedbackMode>(KeySoundFeedback, value);
        }

        /// <summary>
        /// If true, tests will be run in snapshot mode
        /// </summary>
        public bool DisableTestsInSnapMode
        {
            get => GetDataValue<bool>(KeyDisableTestsInSnapMode);
            set => SetDataValue<bool>(KeyDisableTestsInSnapMode, value);
        }

        /// <summary>
        /// if it is true, show the full list of ancestors up to desktop in snap mode.
        /// </summary>
        public bool ShowAncestry
        {
            get => GetDataValue<bool>(KeyShowAncestry);
            set => SetDataValue<bool>(KeyShowAncestry, value);
        }

        /// <summary>
        /// if it is true, do not limit properties list to selected properties
        /// </summary>
        public bool ShowAllProperties
        {
            get => GetDataValue<bool>(KeyShowAllProperties);
            set => SetDataValue<bool>(KeyShowAllProperties, value);
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
            get => GetDataValue<bool>(KeyShowUncertain);
            set => SetDataValue<bool>(KeyShowUncertain, value);
        }

        /// <summary>
        /// Replaces text pattern whitespace with formatting symbols
        /// </summary>
        public bool ShowWhitespaceInTextPatternViewer
        {
            get => GetDataValue<bool>(KeyShowWhitespaceInTextPatternViewer);
            set => SetDataValue<bool>(KeyShowWhitespaceInTextPatternViewer, value);
        }

        /// <summary>
        /// Tree view mode : Raw/Control/Content
        /// default is control
        /// </summary>
        public TreeViewMode TreeViewMode
        {
            get => GetEnumDataValue<TreeViewMode>(KeyTreeViewMode);
            set => SetEnumDataValue<TreeViewMode>(KeyTreeViewMode, value);
        }

        /// <summary>
        /// Inspect scope: Element/Entire App
        /// default is Element
        /// </summary>
        public bool IsUnderElementScope
        {
            get => GetDataValue<bool>(KeyIsUnderElementScope);
            set => SetDataValue<bool>(KeyIsUnderElementScope, value);
        }

        /// <summary>
        /// The release channel configured for this client
        /// </summary>
        public ReleaseChannel ReleaseChannel
        {
            get => GetEnumDataValue<ReleaseChannel>(KeyReleaseChannel);
            set => SetEnumDataValue<ReleaseChannel>(KeyReleaseChannel, value);
        }

        /// <summary>
        /// If true, telemetry will be collected
        /// </summary>
        public bool EnableTelemetry
        {
            get => GetDataValue<bool>(KeyEnableTelemetry);
            set => SetDataValue<bool>(KeyEnableTelemetry, value);
        }

        /// <summary>
        /// If true, telemetry startup dialog will be displayed
        /// </summary>
        public bool ShowTelemetryDialog
        {
            get => GetDataValue<bool>(KeyShowTelemetryDialog);
            set => SetDataValue<bool>(KeyShowTelemetryDialog, value);
        }

        /// <summary>
        /// Check whether we need to show Welcome window or not
        /// </summary>
        /// <returns></returns>
        public bool NeedToShowWelcomeScreen()
        {
            if (ShowWelcomeScreenOnLaunch == false)
            {
                if (this.AppVersion != VersionTools.GetAppVersion())
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
        /// constructor from DiskConfigurationModel
        /// </summary>
        private ConfigurationModel(SettingsDictionary source)
        {
            _settings = new SettingsDictionary(source);
            AppVersion = VersionTools.GetAppVersion();
            Version = CurrentVersion;
        }

        /// <summary>
        /// Copy constructor
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

                config.HotKeyForRecord = hkrecord ?? config.HotKeyForRecord;
                config.HotKeyForPause = hkpause ?? config.HotKeyForPause;
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
                config.RemapSetting(KeyHotKeyLegacyForMoveToNextSibling, KeyHotKeyForMoveToNextSibling);
                config.RemapSetting(KeyHotKeyLegacyForMoveToPreviousSibling, KeyHotKeyForMoveToPreviousSibling);

                // Convert legacy values that are stored as numbers instead of enum names
                config.RemapIntToEnumName<TreeViewMode>(KeyTreeViewMode);
                config.RemapIntToEnumName<HighlighterMode>(KeyHighlighterMode);
                config.RemapIntToEnumName<FontSize>(KeyFontSize);

                // Remap old soundFeedback boolean setting
                if (!config.TryGetValue(KeySoundFeedback, out Object _) && config.TryGetValue(KeyLegacySoundFeedback, out Object value) && (bool)value)
                {
                    config.Add(KeySoundFeedback, SoundFeedbackMode.Always.ToString());
                    // TODO: In a future compatibility-breaking release, remove the PlayScanningSound key from config
                }
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
                SoundFeedback = SoundFeedbackMode.Auto,
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
