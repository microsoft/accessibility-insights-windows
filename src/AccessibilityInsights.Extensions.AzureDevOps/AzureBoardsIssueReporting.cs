// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using AccessibilityInsights.Extensions.AzureDevOps.Models;
using AccessibilityInsights.Extensions.AzureDevOps.FileIssue;
using System.Windows;
using Newtonsoft.Json;

namespace AccessibilityInsights.Extensions.AzureDevOps
{
    [Export(typeof(IIssueReporting))]
    public class AzureBoardsIssueReporting : IIssueReporting
    {
        private AzureDevOpsIntegration AzureDevOps = AzureDevOpsIntegration.GetCurrentInstance();

        private ExtensionConfiguration configuration = new ExtensionConfiguration();

        public bool IsConnected => AzureDevOps.ConnectedToAzureDevOps;

        public string ServiceName => "Azure Boards";

        public Guid StableIdentifier => new Guid("73D8F6EB-E98A-4285-9BA3-B532A7601CC4");

        public bool IsConfigured => false;

        public IEnumerable<byte> Logo => AzureDevOps.Avatar;

        public string LogoText => null;

        public IssueConfigurationControl ConfigurationControl => null;

        public bool CanAttachFiles => true;

        public Task RestoreConfigurationAsync(string serializedConfig)
        {
            if (!String.IsNullOrEmpty(serializedConfig))
            {
                configuration = JsonConvert.DeserializeObject<ExtensionConfiguration>(serializedConfig);
            }

            return HandleLoginAsync();
        }

        public Task<IIssueResult> FileIssueAsync(IssueInformation issueInfo)
        {
            return new Task<IIssueResult>(() => {

                Action<int> updateZoom = (int x) => configuration.ZoomLevel = x;
                (int? bugId, string newBugId) = FileIssueAction.FileNewIssue(issueInfo, configuration.SavedConnection,
                    Application.Current.MainWindow.Topmost, configuration.ZoomLevel, updateZoom);

                // Check whether bug was filed once dialog closed & process accordingly
                if (bugId.HasValue)
                {
                    try
                    {
                        var success = FileIssueAction.AttachIssueData(issueInfo, newBugId, bugId.Value).Result;
                        if (!success)
                        {
                            //MessageDialog.Show(Properties.Resources.HierarchyControl_FileBug_There_was_an_error_identifying_the_created_bug_This_may_occur_if_the_ID_used_to_create_the_bug_is_removed_from_its_Azure_DevOps_description_Attachments_have_not_been_uploaded);
                        }
                        return new IssueResult() { DisplayText = newBugId, IssueLink = null };
                    }
                    catch (Exception)
                    {
                    }
                }
                return null;
            });
        }

        private async Task HandleLoginAsync()
        {
            try
            {
                if (configuration.SavedConnection.ServerUri != null)
                {
                    await FileIssueAction.ConnectAsync(configuration.SavedConnection.ServerUri, false).ConfigureAwait(true);
                    await FileIssueAction.PopulateUserProfileAsync().ConfigureAwait(true);
                }
            }
            catch (Exception)
            {
                FileIssueAction.FlushToken(configuration.SavedConnection.ServerUri);
            }
        }
    }
}
