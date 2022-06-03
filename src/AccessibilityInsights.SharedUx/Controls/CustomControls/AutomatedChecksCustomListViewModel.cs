// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Extensions.Helpers;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.FileIssue;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.ViewModels;
using Axe.Windows.Actions.Contexts;
using Axe.Windows.Core.Bases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    class AutomatedChecksCustomListViewModel
    {
        private readonly ElementDataContext _dataContext;
        private readonly Action _notifyElementSelected;
        private readonly Action _switchToServerLogin;

        internal ElementContext ElementContext { get; set; }

        internal AutomatedChecksCustomListViewModel(ElementDataContext dataContext, Action notifyElementSelected, Action switchToServerLogin)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _notifyElementSelected = notifyElementSelected ?? throw new ArgumentNullException(nameof(notifyElementSelected));
            _switchToServerLogin = switchToServerLogin ?? throw new ArgumentNullException(nameof(switchToServerLogin));
        }

        public void NotifySelected(A11yElement element)
        {
            _dataContext.FocusedElementUniqueId = element.UniqueId;
            _notifyElementSelected();
        }

        /// <summary>
        /// Starts bug fileing for the element identified by the RuleResultViewModel
        /// </summary>
        public void FileBug(RuleResultViewModel vm)
        {
            if (vm.IssueLink != null)
            {
                // Bug already filed, open it in a new window
                try
                {
                    Process.Start(vm.IssueLink.OriginalString);
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
                var telemetryEvent = TelemetryEventFactory.ForIssueFilingRequest(FileBugRequestSource.AutomatedChecks);
                Logger.PublishTelemetryEvent(telemetryEvent);

                if (IssueReporter.IsConnected)
                {
                    IssueInformation issueInformation = vm.GetIssueInformation();
                    FileIssueAction.AttachIssueData(issueInformation, this.ElementContext.Id, vm.Element.BoundingRectangle, vm.Element.UniqueId);

                    IIssueResult issueResult = FileIssueAction.FileIssueAsync(issueInformation);
                    if (issueResult != null)
                    {
                        vm.IssueDisplayText = issueResult.DisplayText;
                        vm.IssueLink = issueResult.IssueLink;
                    }
                    File.Delete(issueInformation.TestFileName);
                }
                else
                {
                    bool? accepted = MessageDialog.Show(Properties.Resources.AutomatedChecksControl_btnFileBug_Click_File_Issue_Configure);
                    if (accepted.HasValue && accepted.Value)
                    {
                        _switchToServerLogin();
                    }
                }
            }
        }
    }
}
