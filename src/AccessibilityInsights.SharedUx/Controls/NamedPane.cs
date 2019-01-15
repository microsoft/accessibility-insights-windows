// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Controls.CustomControls;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Controls
{
    /// <summary>
    /// A "pane" control. Use this as a parent of controls if you want narrator to speak,
    /// "My AutomationProperties.Name pane" before speak the name of the newly focused child control
    /// </summary>
    public class NamedPane : UserControl
    {
        /// <summary>
        /// Identify as pane
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "pane");
        }
    }
}
