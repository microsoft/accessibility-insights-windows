// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Extensions.AzureDevOps.FileIssue;
using AccessibilityInsights.Extensions.AzureDevOps.Models;
using AccessibilityInsights.Extensions.Helpers;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;

namespace AccessibilityInsights.Extensions.AzureDevOps
{
#pragma warning disable RS0034 // Exported parts should have [ImportingConstructor]
    [Export(typeof(IIssueReporting))]
#pragma warning restore RS0034 // Exported parts should have [ImportingConstructor]
    public class AzureBoardsIssueReporting : IIssueReporting
    {
        private readonly FileIssueHelpers _fileIssueHelpers;
        private readonly IDevOpsIntegration _devOpsIntegration;

        /// <summary>
        /// The one and only IDevOpsIntegration object used in production. It's static
        /// so that the ConfigurationControl code can read it. Note that code in this
        /// class reads only the instanced version, not the static version.
        /// </summary>
        internal static IDevOpsIntegration DevOpsIntegration { get; private set; }

        /// <summary>
        /// Production constructor
        /// </summary>
#pragma warning disable RS0034 // Exported parts should have [ImportingConstructor]
        public AzureBoardsIssueReporting()
            : this(new AzureDevOpsIntegration())
        {
        }

        /// <summary>
        /// "Intermediate" production constructor
        /// </summary>
        private AzureBoardsIssueReporting(IDevOpsIntegration devOpsIntegration)
            : this(devOpsIntegration, new FileIssueHelpers(devOpsIntegration))
        {
        }

        /// <summary>
        /// Unit testable constructor
        /// </summary>
        internal AzureBoardsIssueReporting(IDevOpsIntegration devOpsIntegration, FileIssueHelpers fileIssueHelpers)
        {
            _fileIssueHelpers = fileIssueHelpers;
            _devOpsIntegration = devOpsIntegration;
            DevOpsIntegration = devOpsIntegration;
        }
#pragma warning restore RS0034 // Exported parts should have [ImportingConstructor]

        private ExtensionConfiguration Configuration => _devOpsIntegration.Configuration;

        public bool IsConnected => _devOpsIntegration.ConnectedToAzureDevOps;

        public string ServiceName { get; } = Properties.Resources.ServiceName;

        public Guid StableIdentifier { get; } = new Guid("73D8F6EB-E98A-4285-9BA3-B532A7601CC4");

        public bool IsConfigured => _devOpsIntegration.ConnectedToAzureDevOps
            && Configuration?.SavedConnection?.IsPopulated == true
            && _devOpsIntegration.CheckIfAbleToGetProjects().Result;

        public ReporterFabricIcon Logo => ReporterFabricIcon.VSTSLogo;

        public string ConfigurationPath { get; private set; }

        public string LogoText => Properties.Resources.LogoText;

        public IssueConfigurationControl ConfigurationControl { get; } = new ConfigurationControl();

        public bool CanAttachFiles => true;

        public Task RestoreConfigurationAsync(string serializedConfig)
        {
            if (!String.IsNullOrEmpty(serializedConfig))
            {
                Configuration.LoadFromSerializedString(serializedConfig);
            }

            return _devOpsIntegration.HandleLoginAsync();
        }

        public Task<IIssueResultWithPostAction> FileIssueAsync(IssueInformation issueInfo)
        {
            bool topMost = false;
            Application.Current.Dispatcher.Invoke(() => topMost = Application.Current.MainWindow.Topmost);

            Action<int> updateZoom = (int x) => Configuration.ZoomLevel = x;
            (int? issueId, string newIssueId) = _fileIssueHelpers.FileNewIssue(issueInfo, Configuration.SavedConnection,
                topMost, Configuration.ZoomLevel, updateZoom, this.ConfigurationPath);

            return Task.Run<IIssueResultWithPostAction>(() =>
            {
                // Check whether issue was filed once dialog closed & process accordingly
                if (!issueId.HasValue)
                    return null;

                try
                {
                    if (!_fileIssueHelpers.AttachIssueData(issueInfo, newIssueId, issueId.Value).Result)
                    {
                        return new IssueResultWithPostAction()
                        {
                            DisplayText = null,
                            IssueLink = null,
                            PostAction = () =>
                            {
                                MessageDialog.Show(Properties.Resources.There_was_an_error_identifying_the_created_issue_This_may_occur_if_the_ID_used_to_create_the_issue_is_removed_from_its_Azure_DevOps_description_Attachments_have_not_been_uploaded);
                            },
                        };
                    }

                    return new IssueResultWithPostAction()
                    {
                        DisplayText = issueId.ToString(),
                        IssueLink = _devOpsIntegration.GetExistingIssueUrl(issueId.Value),
                        PostAction = null,
                    };
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
                {
                    e.ReportException();
                }
#pragma warning restore CA1031 // Do not catch general exception types

                return null;
            });
        }

        public IssueConfigurationControl RetrieveConfigurationControl(Action UpdateSaveButton)
        {
            ConfigurationControl.UpdateSaveButton = UpdateSaveButton;
            return ConfigurationControl;
        }

        public bool TryGetCurrentSerializedSettings(out string settings)
        {
            settings = _devOpsIntegration?.Configuration?.GetSerializedConfig();
            return settings != null;
        }

        public void SetConfigurationPath(string configurationPath)
        {
            this.ConfigurationPath = configurationPath;
        }
    }
}
