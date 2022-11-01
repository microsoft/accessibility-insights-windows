using System;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Automation.Provider;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    internal class CheckableTreeViewItemAutomationPeer : TreeViewItemAutomationPeer
    {
        private CheckBox _checkBox;

        public CheckableTreeViewItemAutomationPeer(TreeViewItem owner, CheckBox checkBox)
            : base(owner)
        {
            if (checkBox == null) throw new ArgumentNullException(nameof(checkBox));

            _checkBox = checkBox;
        }

        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new CheckableTreeViewDataItemAutomationPeer(item, this, base.EventsSource as CheckableTreeViewDataItemAutomationPeer, _checkBox);
        }
    }
}
