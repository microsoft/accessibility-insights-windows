// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using System;

namespace AccessibilityInsights.SharedUx.Interfaces
{
    internal interface IIssueFilingSource
    {
        /// <summary>
        /// Element
        /// </summary>
        A11yElement Element { get; }

        /// <summary>
        /// Used to store the issue link.
        /// </summary>
        Uri IssueLink { get; set; }

        /// <summary>
        /// Bug id of this element
        /// </summary>
        string IssueDisplayText { get; set; }
    }
}
