// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Helpers;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.FileIssue;
using AccessibilityInsights.SharedUx.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

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
        /// Delegates
        /// </summary>
        public Action UpdateSaveButton { get; set; }
        public Action<bool> ShowSaveButton { get; set; }

        IssueConfigurationControl issueConfigurationControl;
        IIssueReporting selectedIssueReporter;

        #region configuration updating code

        private RadioButton CreateRadioButton(IIssueReporting reporter)
        {
            RadioButton issueReportingOption = new RadioButton();
            issueReportingOption.Style = FindResource("primaryFGRadioButtonStyle") as Style;
            issueReportingOption.Content = reporter.ServiceName;
            issueReportingOption.Tag = reporter.StableIdentifier;
            issueReportingOption.Margin = new Thickness(2, 2, 2, 2);
            issueReportingOption.Checked += IssueReporterOnChecked;
            issueReportingOption.FontSize = 14;
            return issueReportingOption;
        }

        private void IssueReporterOnChecked(object sender, RoutedEventArgs e)
        {
            if (issueConfigurationControl != null)
            {
                issueFilingGrid.Children.Remove(issueConfigurationControl);
                issueConfigurationControl = null;
                UpdateSaveButton();
            }

            Guid clickedOptionTag = (Guid)((RadioButton)sender).Tag;
            if (clickedOptionTag != Guid.Empty)
            {
                IssueReporterManager.GetInstance().IssueFilingOptionsDict.TryGetValue(clickedOptionTag, out selectedIssueReporter);
                issueConfigurationControl = selectedIssueReporter?.RetrieveConfigurationControl(this.UpdateSaveButton);
                Grid.SetRow(issueConfigurationControl, 3);
                issueFilingGrid.Children.Add(issueConfigurationControl);
            }
            UpdateSaveButton();
        }

        /// <summary>
        /// Adds the currently selected connection to the configuration so it is persisted
        /// </summary>
        /// <param name="configuration"></param>
        public bool UpdateConfigFromSelections(ConfigurationModel configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // For first time / valid reconfigurations canSave will be enabled and hence config will be saved. Else only set the reporter.
            if (issueConfigurationControl != null && (issueConfigurationControl.CanSave || selectedIssueReporter.IsConfigured))
            {
                configuration.SelectedIssueReporter = selectedIssueReporter.StableIdentifier;

                if (issueConfigurationControl.CanSave)
                {
                    string serializedConfigs = configuration.IssueReporterSerializedConfigs;

                    Dictionary<Guid, string> configs = new Dictionary<Guid, string>();
                    try
                    {
                        Dictionary<Guid, string> parsedConfigs = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(serializedConfigs);
                        configs = parsedConfigs ?? configs;
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception e)
                    {
                        e.ReportException();
                    }
#pragma warning restore CA1031 // Do not catch general exception types

                    string newConfigs = issueConfigurationControl.OnSave();
                    configs[selectedIssueReporter.StableIdentifier] = newConfigs;
                    configuration.IssueReporterSerializedConfigs = JsonConvert.SerializeObject(configs);
                }
                IssueReporterManager.GetInstance().SetIssueReporter(selectedIssueReporter.StableIdentifier);
                IssueReporter.SetConfigurationPath(ConfigurationManager.GetDefaultInstance().SettingsProvider.ConfigurationFolderPath);
                issueConfigurationControl.OnDismiss();
                return true;
            }
            return false;
        }

        /// <summary>
        /// For this control we want SaveAndClose to be enabled if the extension control indicates that something can be saved.
        /// </summary>
        public bool IsConfigurationChanged(ConfigurationModel configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (issueConfigurationControl != null)
            {
                bool hasReporterChanged = configuration.SelectedIssueReporter != selectedIssueReporter.StableIdentifier;
                if (hasReporterChanged)
                {
                    return issueConfigurationControl.CanSave || selectedIssueReporter.IsConfigured;
                }
                return issueConfigurationControl.CanSave;
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Inititates the view. Fetches a list of all the available issue reporters and creates a list.
        /// </summary>
        public void InitializeView()
        {
            IReadOnlyDictionary<Guid, IIssueReporting> options = IssueReporterManager.GetInstance().IssueFilingOptionsDict;
            availableIssueReporters.Children.Clear();
            Guid selectedGUID = IssueReporter.IssueReporting != null ? IssueReporter.IssueReporting.StableIdentifier : default(Guid);
            foreach (var reporter in options)
            {
                if (reporter.Key == Guid.Empty || reporter.Value == null)
                    continue;

                RadioButton rb = CreateRadioButton(reporter.Value);
                if (selectedGUID.Equals(reporter.Key))
                {
                    rb.IsChecked = true;
                    issueConfigurationControl = reporter.Value.RetrieveConfigurationControl(this.UpdateSaveButton);
                    Grid.SetRow(issueConfigurationControl, 3);
                    if (!issueFilingGrid.Children.Contains(issueConfigurationControl))
                    {
                        issueFilingGrid.Children.Add(issueConfigurationControl);
                    }
                }
                availableIssueReporters.Children.Add(rb);
            }

            this.issueFilingGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Go to link
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}