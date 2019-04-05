// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;

namespace AccessibilityInsights.SharedUx.Interfaces
{
    /// <summary>
    /// Interface defining methods used by HierarchyControl
    /// </summary>
    public interface IHierarchyAction
    {
        /// <summary>
        /// Refresh hierarchy by clearing data context
        /// </summary>
        void RefreshHierarchy(bool newData);

        /// <summary>
        /// Action to perform when user needs to log into the bug server
        /// </summary>
        void SwitchToServerLogin();

        /// <summary>
        /// Start event mode with given element
        /// </summary>
        void HandleLiveToEvents(A11yElement el);

        /// <summary>
        /// Run tests, switch to test results in UIA tree, open file bug dialog
        /// </summary>
        void HandleFileBugLiveMode();

        /// <summary>
        /// Action to take when hierarchy selection changes
        /// </summary>
        void SelectedElementChanged();
    }
}
