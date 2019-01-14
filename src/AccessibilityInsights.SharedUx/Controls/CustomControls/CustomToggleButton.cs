// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    public class CustomToggleButton : ToggleButton
    {
        /// <summary>
        /// Class to override AutomationPeer for the purpose of removing the toggle button from the UIA tree
        /// </summary>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return null;
        }
    }
}
