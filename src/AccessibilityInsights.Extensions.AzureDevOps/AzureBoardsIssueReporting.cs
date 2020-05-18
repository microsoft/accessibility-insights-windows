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
    [Export(typeof(IIssueReporting))]
    public class AzureBoardsIssueReporting : IIssueReporting
    {
        private readonly FileIssueHelpers _fileIssueHelpers;
        private readonly IDevOpsIntegration _devOpsIntegration;

        /// <summary>
        /// Production ctor
        /// </summary>
        public AzureBoardsIssueReporting()
            : this(AzureDevOpsIntegration.GetCurrentInstance())
        {
        }

        /// <summary>
        /// "Intermediate" production ctor
        /// </summary>
        private AzureBoardsIssueReporting(IDevOpsIntegration devOpsIntegration)
            : this(devOpsIntegration, new FileIssueHelpers(devOpsIntegration))
        {
        }

        /// <summary>
        /// Unit testable ctor
        /// </summary>
        internal AzureBoardsIssueReporting(IDevOpsIntegration devOpsIntegration, FileIssueHelpers fileIssueHelpers)
        {
            _fileIssueHelpers = fileIssueHelpers;
            _devOpsIntegration = devOpsIntegration;
        }

        private ExtensionConfiguration Configuration => _devOpsIntegration.Configuration;

        public bool IsConnected => _devOpsIntegration.ConnectedToAzureDevOps;

        public string ServiceName { get; } = "Azure Boards";

        public Guid StableIdentifier { get; } = new Guid("73D8F6EB-E98A-4285-9BA3-B532A7601CC4");

        public bool IsConfigured => _devOpsIntegration.ConnectedToAzureDevOps;

        public ReporterFabricIcon Logo => ReporterFabricIcon.VSTSLogo;

        public string LogoText => "Azure Boards";

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

        public Task<IIssueResult> FileIssueAsync(IssueInformation issueInfo)
        {
            bool topMost = false;
            Application.Current.Dispatcher.Invoke(() => topMost = Application.Current.MainWindow.Topmost);

            Action<int> updateZoom = (int x) => Configuration.ZoomLevel = x;
            (int? issueId, string newIssueId) = _fileIssueHelpers.FileNewIssue(issueInfo, Configuration.SavedConnection,
                topMost, Configuration.ZoomLevel, updateZoom);

            return Task.Run<IIssueResult>(() => {
                // Check whether issue was filed once dialog closed & process accordingly
                if (!issueId.HasValue) return null;

                try
                {
                    if (!_fileIssueHelpers.AttachIssueData(issueInfo, newIssueId, issueId.Value).Result)
                    {
                        MessageDialog.Show(Properties.Resources.There_was_an_error_identifying_the_created_issue_This_may_occur_if_the_ID_used_to_create_the_issue_is_removed_from_its_Azure_DevOps_description_Attachments_have_not_been_uploaded);
                    }

                    return new IssueResult()
                    {
                        DisplayText = issueId.ToString(),
                        IssueLink = _devOpsIntegration.GetExistingIssueUrl(issueId.Value)
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
    }
}
