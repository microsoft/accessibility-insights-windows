// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Desktop.UIAutomation;
using AccessibilityInsights.SetupLibrary;
using System;
using System.IO;
using System.Windows;
using System.Linq;
using System.Collections.Generic;

namespace AccessibilityInsights.SharedUx.Settings
{
    /// <summary>
    /// Configuration Manager class
    /// it handles all configuration related operation and maintain default configuration. 
    /// </summary>
    public class ConfigurationManager
    {
        /// <summary>
        /// Application Configuration
        /// </summary>
        public ConfigurationModel AppConfig { get; private set; }

        /// <summary>
        /// Application layout
        /// </summary>
        public AppLayout AppLayout { get; private set; }

        /// <summary>
        /// Event recorder settings
        /// </summary>
        public RecorderSetting EventConfig { get; private set; }

        /// <summary>
        /// private constructor
        /// </summary>
        private ConfigurationManager()
        {
            PopulateMainConfiguration();
            PopulateEventConfiguration();
            PopulateLayout();
        }

        /// <summary>
        /// Save all configuration. 
        /// - AppConfig
        /// - Layout
        /// </summary>
        public void Save()
        {
            try
            {
                var fp = Path.Combine(DirectoryManagement.sConfigurationFolderPath, LayoutFileName);
                this.AppLayout.SerializeInJSON(fp);

                fp = Path.Combine(DirectoryManagement.sConfigurationFolderPath, SetupLibrary.Constants.AppConfigFileName);
                this.AppConfig.SerializeInJSON(fp);
            }
            catch
            {
                // fail silently since it is called at the end of the app life cycle. 
            }
        }

        /// <summary>
        /// Populate Layout info
        /// </summary>
        private void PopulateLayout()
        {
            var fp = Path.Combine(DirectoryManagement.sConfigurationFolderPath, LayoutFileName);
            var window = Application.Current.MainWindow;

            // Layout
            try
            {
                this.AppLayout = AppLayout.LoadFromJSON<AppLayout>(fp);

                this.AppLayout.LoadLayoutIfPrevVersion(window.Top, window.Left);
            }
            catch (Exception)
            {
                AppLayout.RemoveConfiguration(fp);
                this.AppLayout = new AppLayout(window.Top, window.Left);
                this.AppLayout.SerializeInJSON(fp);
            }
        }

        /// <summary>
        /// Populate main configuration
        /// </summary>
        private void PopulateMainConfiguration()
        {
            var fp = Path.Combine(DirectoryManagement.sConfigurationFolderPath, SetupLibrary.Constants.AppConfigFileName);

            // Main configuration 
            try
            {
                this.AppConfig = ConfigurationModel.LoadFromJSON(fp);
            }
            catch (Exception)
            {
                ConfigurationModel.RemoveConfiguration(fp);
                this.AppConfig = ConfigurationModel.GetDefaultConfigurationModel();
                this.AppConfig.SerializeInJSON(fp);
            }

            DesktopElementHelper.SetCorePropertiesList(this.AppConfig.CoreProperties);
        }

        /// <summary>
        /// Populate event configuration
        /// </summary>
        public void PopulateEventConfiguration()
        {
            var configpath = Path.Combine(DirectoryManagement.sUserDataFolderPath, EventConfigFileName);
            try
            {
                var rcfg = LoadConfiguration(configpath);
                this.EventConfig = rcfg;

            }
            catch (Exception)
            {
                RecorderSetting.RemoveConfiguration(configpath);
                this.EventConfig = LoadConfiguration(configpath);
            }
        }

        public static RecorderSetting LoadConfiguration(string path)
        {
            // Get Recorder configuration from local location. but if it is not available, get it from default location. 
            RecorderSetting config = new RecorderSetting();
            config = RecorderSetting.LoadFromJSON<RecorderSetting>(path);
            if (config == null)
            {
                config = GetDefaultRecordingConfig();
                config.SerializeInJSON(path);
            }
            else
            {
                // check whether there is any new events to be added into configuration. 
                var events = Axe.Windows.Desktop.Types.EventType.GetInstance();
                var ms = from e in events.GetKeyValuePairList()
                         where IsNotInList(e.Key, config.Events)
                         select e;

                if (ms.Count() != 0)
                {
                    foreach (var m in ms)
                    {
                        config.Events.Add(new RecordEntitySetting()
                        {
                            Id = m.Key,
                            Name = m.Value,
                            IsRecorded = false,
                            Type = RecordEntityType.Event,
                        });
                    }
                    config.SerializeInJSON(path);
                }
                config.IsListeningAllEvents = false;
            }

            return config;
        }

        /// <summary>
        /// check whether key exist in the given list. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="events"></param>
        /// <returns></returns>
        private static bool IsNotInList(int key, List<RecordEntitySetting> events)
        {
            return (from e in events
                    where e.Id == key
                    select e).Count() == 0;
        }

        /// <summary>
        /// Get the default Event Recording Configuration
        /// </summary>
        /// <returns></returns>
        public static RecorderSetting GetDefaultRecordingConfig()
        {
            RecorderSetting config = new RecorderSetting
            {
                // Set Individual Event
                Events = (from e in Axe.Windows.Desktop.Types.EventType.GetInstance().GetKeyValuePairList()
                          select new RecordEntitySetting()
                          {
                              Type = RecordEntityType.Event,
                              Id = e.Key,
                              Name = e.Value,
                              IsRecorded = false
                          }).ToList(),

                // Set properties
                Properties = (from e in Axe.Windows.Core.Types.PropertyType.GetInstance().GetKeyValuePairList()
                              select new RecordEntitySetting()
                              {
                                  Type = RecordEntityType.Property,
                                  Id = e.Key,
                                  Name = e.Value,
                                  IsRecorded = false
                              }).ToList(),

                // Set Global event
                IsListeningFocusChangedEvent = true,

                // Individual Event Scope
                ListenScope = UIAutomationClient.TreeScope.TreeScope_Subtree
            };

            return config;
        }

        /// <summary>
        /// Save events configuration
        /// </summary>
        public void SaveEventsConfiguration()
        {
            var configpath = Path.Combine(DirectoryManagement.sUserDataFolderPath, EventConfigFileName);

            this.EventConfig.SerializeInJSON(configpath);
        }

        #region static members
        /// <summary>
        /// Default configuration manager
        /// </summary>
        static ConfigurationManager sDefaultInstance = null;

        /// <summary>
        /// Get the default instance of ConfigurationManager
        /// </summary>
        /// <returns></returns>
        public static ConfigurationManager GetDefaultInstance()
        {
            if(sDefaultInstance == null)
            {
                try
                {
                    sDefaultInstance = new ConfigurationManager();
                }
                catch
                {
                    // be silent. since it will be ok later once Main window is up.
                }
            }

            return sDefaultInstance;
        }

        /// <summary>
        /// Layout config file name
        /// </summary>
        const string LayoutFileName = "Layout.Json";

        /// <summary>
        /// Event config file name
        /// </summary>
        const string EventConfigFileName = "EventConfig.Json";
        #endregion
    }
}
