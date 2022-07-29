// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    public class CustomExpander : Expander
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomExpanderAutomationPeer(this);
        }
    }

    public class CustomExpanderAutomationPeer : ExpanderAutomationPeer
    {
        readonly GroupItem parentGroupItem;

        public CustomExpanderAutomationPeer(Expander owner) : base(owner) => parentGroupItem = owner?.TemplatedParent as GroupItem;

        /// <summary>
        /// In .NET 4.8 grouped ListViews, the GroupItem's HasKeyboardFocus value is set based on its child expander's
        /// HasKeyboardFocus value, not its own IsFocused value. In our AutomatedChecks use case, this breaks ATs'
        /// readings of the ListView group headers. By setting the child expander's HasKeyboardFocus value here,
        /// we get the desired value in its parent GroupItem's value, and ATs function properly again.
        /// </summary>
        protected override bool HasKeyboardFocusCore() => parentGroupItem?.IsFocused ?? base.HasKeyboardFocusCore();
    }
}
