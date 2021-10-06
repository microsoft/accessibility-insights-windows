// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.Extensions.Interfaces.IssueReporting
{
    /// <summary>
    /// Interface for the details displayed on the new issue button after filing is complete
    /// </summary>
    public interface IIssueResult
    {
        /// <summary>
        /// The Display text for the filed issue
        /// </summary>
        string DisplayText { get; }

        /// <summary>
        /// The link for the filed issue
        /// </summary>
        Uri IssueLink { get; }
    }

    public interface IIssueResultWithPostAction : IIssueResult
    {
        /// <summary>
        /// Action to run on UI thread after issue filing is completed
        /// </summary>
        Action PostAction { get; }
    }
}
