// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows.Controls;

namespace AccessibilityInsights.Extensions.Interfaces.IssueReporting
{
    /// <summary>
    /// Control to display in Connection tab of Configuration page
    /// </summary>
    public abstract class IssueConfigurationControl : UserControl
    {
        /// <summary>
        /// Can the save button be clicked
        /// </summary>
        public bool CanSave { get; }

        /// <summary>
        /// Called when save button clicked.
        /// </summary>
        /// <returns>The extension’s new configuration, serialized</returns>
        public abstract string OnSave();

        /// <summary>
        /// Called when settings page dismissed
        /// </summary>
        public abstract void OnDismiss();
    }
}
