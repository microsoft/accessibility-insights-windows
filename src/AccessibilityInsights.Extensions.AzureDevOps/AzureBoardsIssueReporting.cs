// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.AzureDevOps.FileIssue;
using AccessibilityInsights.Extensions.AzureDevOps.Models;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;

namespace AccessibilityInsights.Extensions.AzureDevOps
{
    [Export(typeof(IIssueReporting))]
    public class AzureBoardsIssueReporting : IIssueReporting
    {
        private static AzureDevOpsIntegration AzureDevOps => AzureDevOpsIntegration.GetCurrentInstance();

        private static ExtensionConfiguration Configuration => AzureDevOps.Configuration;

        public static bool IsConnected => AzureDevOps.ConnectedToAzureDevOps;

        public string ServiceName { get; } = "Azure Boards";

        public Guid StableIdentifier { get; } = new Guid("73D8F6EB-E98A-4285-9BA3-B532A7601CC4");

        public bool IsConfigured => AzureDevOps.ConnectedToAzureDevOps;

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

            return AzureDevOps.HandleLoginAsync();
        }

        public Task<IIssueResult> FileIssueAsync(IssueInformation issueInfo)
        {
            bool topMost = false;
            Application.Current.Dispatcher.Invoke(() => topMost = Application.Current.MainWindow.Topmost);

            Action<int> updateZoom = (int x) => Configuration.ZoomLevel = x;
            (int? issueId, string newIssueId) = FileIssueHelpers.FileNewIssue(issueInfo, Configuration.SavedConnection,
                topMost, Configuration.ZoomLevel, updateZoom);

            return Task.Run<IIssueResult>(() => {
                // Check whether issue was filed once dialog closed & process accordingly
                if (!issueId.HasValue) return null;

                try
                {
                    if (!FileIssueHelpers.AttachIssueData(issueInfo, newIssueId, issueId.Value).Result)
                    {
                        //MessageDialog.Show(Properties.Resources.HierarchyControl_FileIssue_There_was_an_error_identifying_the_created_issue_This_may_occur_if_the_ID_used_to_create_the_issue_is_removed_from_its_Azure_DevOps_description_Attachments_have_not_been_uploaded);
                    }
                    return new IssueResult() { DisplayText = newIssueId, IssueLink = null };
                }
                catch (Exception)
                {
                }

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
