// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Interfaces;
using System;

namespace AccessibilityInsights.SharedUx.Controls
{
    /// <summary>
    /// Holds inputs to the FileIssueWrapper.FileIssueFromControl method. Allows us to
    /// decouple the control specifics from the shared code
    /// </summary>
    internal class FileIssueWrapperInput
    {
        internal IIssueFilingSource VM { get; }
        internal Guid EcId { get; }
        internal Action SwitchToServerLogin { get; }
        internal Func<IssueInformation> IssueInformationProvider { get; }
        internal FileBugRequestSource RequestSource { get; }

        internal FileIssueWrapperInput(IIssueFilingSource vm, Guid ecId, Action switchToServerLogin,
            Func<IssueInformation> issueInformationProvider, FileBugRequestSource requestSource)
        {
            VM = vm ?? throw new ArgumentNullException(nameof(vm));
            EcId = ecId;
            SwitchToServerLogin = switchToServerLogin ?? throw new ArgumentNullException(nameof(switchToServerLogin));
            IssueInformationProvider = issueInformationProvider ?? throw new ArgumentNullException(nameof(issueInformationProvider));
            RequestSource = requestSource;
        }
    }
}
