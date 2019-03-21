// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.FileBug;
using AccessibilityInsights.SharedUx.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using AccessibilityInsights.SharedUx.Properties;


namespace AccessibilityInsights.SharedUx.Controls.SettingsTabs
{
    /// <summary>
    /// Interaction logic for ConnectionControl.xaml
    /// </summary>
    public partial class ConnectionControl : UserControl
    {
        public ConnectionControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Represents different states for whether user has connected to server yet
        /// </summary>
        private enum States
        {
            NoServer,       // First screen with "next"
            EditingServer,  // Second screen with treeview
            HasServer       // Third screen with selected team
        };

        /// <summary>
        /// Delegates
        /// </summary>
        public Action UpdateSaveButton { get; set; }
        public Action<Uri, bool, Action> HandleLoginRequest { get; set; }
        public Action<Action> HandleLogoutRequest { get; set; }
        public Action<bool> ShowSaveButton { get; set; }

        IssueConfigurationControl issueConfigurationControl = null;
        IIssueReporting selectedIssueReporter = null;

        #region configuration updating code
        /// <summary>
        /// Initializes the view.
        /// </summary>
        /// <param name="configuration"></param>
        public void UpdateFromConfig()
        {
                InitializeView();
        }

        private RadioButton CreateRadioButton(IIssueReporting reporter)
        {
            RadioButton issueReportingOption = new RadioButton();
            issueReportingOption.Content = reporter.ServiceName;
            issueReportingOption.Tag =reporter.StableIdentifier;
            issueReportingOption.Margin = new Thickness(2, 2, 2, 2);
            issueReportingOption.Checked += IssueReporterOnChecked;
            issueReportingOption.FontSize = 14;
            return issueReportingOption;
        }

        private void IssueReporterOnChecked(object sender, RoutedEventArgs e)
        {
            if (issueConfigurationControl != null) {
                selectServerGrid.Children.Remove(issueConfigurationControl);
                issueConfigurationControl = null;
                UpdateSaveButton();
            }

            Guid clickedButton  = (Guid)((RadioButton)sender).Tag;
            IssueReporterManager.GetInstance().GetIssueFilingOptionsDict().TryGetValue(clickedButton, out selectedIssueReporter);
            issueConfigurationControl = selectedIssueReporter.RetrieveConfigurationControl(this.UpdateSaveButton);
            Grid.SetRow(issueConfigurationControl, 3);
            selectServerGrid.Children.Add(issueConfigurationControl);
        }

        /// <summary>
        /// Adds the currently selected connection to the configuration so it is persisted
        /// </summary>
        /// <param name="configuration"></param>
        public bool UpdateConfigFromSelections(ConfigurationModel configuration)
        {
            if (issueConfigurationControl.CanSave)
            {
                configuration.SelectedIssueReporter = selectedIssueReporter.StableIdentifier;
                string serializedConfigs = configuration.IssueReporterSerializedConfigs;
                Dictionary<Guid, string> configs = new Dictionary<Guid, string>();

                if (serializedConfigs != null)
                {
                    configs = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(serializedConfigs);
                }

                string newConfigs = issueConfigurationControl.OnSave();
                configs[selectedIssueReporter.StableIdentifier] = newConfigs;
                configuration.IssueReporterSerializedConfigs = JsonConvert.SerializeObject(configs);
                IssueReporterManager.GetInstance().SetIssueReporter(selectedIssueReporter.StableIdentifier);
                issueConfigurationControl.OnDismiss();
                return true;
            }
            return false;
        }

        /// <summary>
        /// For this control we want SaveAndClose to be enabled if the extension control indicates that something can be saved.
        /// </summary>
        public bool IsConfigurationChanged()
        {
            return issueConfigurationControl != null ? issueConfigurationControl.CanSave : false;
        }
        #endregion

        /// <summary>
        /// Inititates the view. Fetches a list of all the available issue reporters and creates a list.
        /// </summary>
        public void InitializeView()
        {
            IReadOnlyDictionary<Guid, IIssueReporting> options = IssueReporterManager.GetInstance().GetIssueFilingOptionsDict();
            availableIssueReporters.Children.Clear();
            Guid selectedGUID = BugReporter.IssueReporting != null ? BugReporter.IssueReporting.StableIdentifier : default(Guid);
            foreach (var reporter in options)
            {
                RadioButton rb = CreateRadioButton(reporter.Value);
                if (reporter.Key.Equals(selectedGUID))
                {
                    rb.IsChecked = true;
                    issueConfigurationControl = reporter.Value.RetrieveConfigurationControl(this.UpdateSaveButton);
                    Grid.SetRow(issueConfigurationControl, 3);
                    selectServerGrid.Children.Add(issueConfigurationControl);
                }
                availableIssueReporters.Children.Add(rb);
            }

            this.selectServerGrid.Visibility = Visibility.Visible;
        }
    }
}