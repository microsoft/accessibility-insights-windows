// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    internal class CheckableTreeViewItem : TreeViewItem
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CheckableTreeViewItemAutomationPeer(this);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CheckableTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is CheckableTreeViewItem;
        }
    }
}
