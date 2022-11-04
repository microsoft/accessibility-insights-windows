using System;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    internal class CheckableTreeViewItemAutomationPeer : TreeViewItemAutomationPeer
    {
        public CheckableTreeViewItemAutomationPeer(TreeViewItem owner) : base(owner) {}

        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new CheckableTreeViewDataItemAutomationPeer(item, this, base.EventsSource as CheckableTreeViewDataItemAutomationPeer);
        }
    }
}
