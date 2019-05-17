// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Desktop.UIAutomation;
using AccessibilityInsights.SetupLibrary;
using AccessibilityInsights.SharedUx.Telemetry;
using System;
using System.IO;
using System.Windows;

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
#pragma warning disable CA1031 // Do not catch general exception types
            catch
            {
                // fail silently since it is called at the end of the app life cycle. 
            }
#pragma warning restore CA1031 // Do not catch general exception types
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
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                e.ReportException();
                AppLayout.RemoveConfiguration(fp);
                this.AppLayout = new AppLayout(window.Top, window.Left);
                this.AppLayout.SerializeInJSON(fp);
            }
#pragma warning restore CA1031 // Do not catch general exception types
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
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                e.ReportException();
                ConfigurationModel.RemoveConfiguration(fp);
                this.AppConfig = ConfigurationModel.GetDefaultConfigurationModel();
                this.AppConfig.SerializeInJSON(fp);
            }
#pragma warning restore CA1031 // Do not catch general exception types

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
                var rcfg = RecorderSetting.LoadConfiguration(configpath);
                this.EventConfig = rcfg;

            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                e.ReportException();
                RecorderSetting.RemoveConfiguration(configpath);
                this.EventConfig = RecorderSetting.LoadConfiguration(configpath);
            }
#pragma warning restore CA1031 // Do not catch general exception types
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
#pragma warning disable CA1031 // Do not catch general exception types
                catch
                {
                    // be silent. since it will be ok later once Main window is up.
                }
#pragma warning restore CA1031 // Do not catch general exception types
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
