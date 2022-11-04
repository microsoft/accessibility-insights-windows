using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    internal class CheckableTreeViewItem : TreeViewItem
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CheckableTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is CheckableTreeViewItem;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CheckableTreeViewItemAutomationPeer(this);
        }
    }
}
