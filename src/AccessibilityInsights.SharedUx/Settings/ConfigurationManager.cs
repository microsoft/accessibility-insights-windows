// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Desktop.Settings;
using AccessibilityInsights.Desktop.UIAutomation;
using AccessibilityInsights.RuleSelection;
using AccessibilityInsights.SetupLibrary;
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
        /// TestConfiguration
        /// </summary>
        public TestSetting TestConfig { get; set; }

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
            PopulateTestConfiguration();
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
        /// Populate Test Configuration
        /// </summary>
        private void PopulateTestConfiguration()
        {
            // Test configuration
            SuiteConfigurationType configType = this.AppConfig.TestConfig;
            this.TestConfig = TestSetting.GenerateSuiteConfiguration(configType);
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
                var rcfg = RecorderSetting.LoadConfiguration(configpath);
                this.EventConfig = rcfg;

            }
            catch (Exception)
            {
                RecorderSetting.RemoveConfiguration(configpath);
                this.EventConfig = RecorderSetting.LoadConfiguration(configpath);
            }
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
