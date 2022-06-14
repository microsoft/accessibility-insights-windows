// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Extensions.Helpers;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.FileIssue;
using AccessibilityInsights.SharedUx.Interfaces;
using AccessibilityInsights.SharedUx.Telemetry;
using System;
using System.IO;

namespace AccessibilityInsights.SharedUx.Controls
{
    /// <summary>
    /// Simple class to consolidate control-related code for issue filing into a single place
    /// </summary>
    internal static class FileIssueWrapper
    {
        /// <summary>
        /// Handles control-related issue filing
        /// </summary>
        internal static void FileIssueFromControl(FileIssueWrapperInput input)
        {
            IIssueFilingSource vm = input.VM;
            if (vm.IssueLink != null)
            {
                // Bug already filed, open it in a new window
                try
                {
                    System.Diagnostics.Process.Start(vm.IssueLink.OriginalString);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
                {
                    ex.ReportException();
                    // Happens when bug is deleted, message describes that work item doesn't exist / possible permission issue
                    MessageDialog.Show(ex.InnerException?.Message);
                    vm.IssueDisplayText = null;
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
            else
            {
                // File a new bug
                var telemetryEvent = TelemetryEventFactory.ForIssueFilingRequest(input.RequestSource);
                Logger.PublishTelemetryEvent(telemetryEvent);

                if (IssueReporter.IsConnected)
                {
                    IssueInformation issueInformation = null;
                    try
                    {
                        issueInformation = input.IssueInformationProvider();
                        FileIssueAction.AttachIssueData(issueInformation, input.EcId, vm.Element.BoundingRectangle, vm.Element.UniqueId);
                        IIssueResult issueResult = FileIssueAction.FileIssueAsync(issueInformation);
                        if (issueResult != null)
                        {
                            vm.IssueDisplayText = issueResult.DisplayText;
                            vm.IssueLink = issueResult.IssueLink;
                        }
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception ex)
                    {
                        ex.ReportException();
                    }
#pragma warning restore CA1031 // Do not catch general exception types
                    finally
                    {
                        if (issueInformation != null && File.Exists(issueInformation.TestFileName))
                        {
                            File.Delete(issueInformation.TestFileName);
                        }
                    }
                }
                else
                {
                    bool? accepted = MessageDialog.Show(Properties.Resources.FileIssuesChooseLocation);
                    if (accepted.HasValue && accepted.Value)
                    {
                        input.SwitchToServerLogin();
                    }
                }
            }

        }
    }
}
