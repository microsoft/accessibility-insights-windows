using System;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows;
using AccessibilityInsights.SharedUx.ViewModels;
using System.Windows.Media;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    internal class CheckableTreeViewItemAutomationPeer : TreeViewItemAutomationPeer
    {
        private TreeViewItem _owner;

        public CheckableTreeViewItemAutomationPeer(TreeViewItem owner)
            : base(owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));

            _owner = owner;
        }

        private static CheckBox FindMatchingCheckBox(DependencyObject parent, EventConfigNodeViewModel evm)
        {
            if (parent == null) return null;

            var childCount = VisualTreeHelper.GetChildrenCount(parent);
            if (childCount < 1) return null;

            var candidateChild = VisualTreeHelper.GetChild(parent, 0);
            if (candidateChild is CheckBox)
            {
                var text = (VisualTreeHelper.GetChild(parent, 1) as TextBlock).Text;
                if (text == evm.Header)
                {
                    return candidateChild as CheckBox;
                }
            }

            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var descendant = FindMatchingCheckBox(child, evm);
                if (descendant != null)
                    return descendant;
            }

            return null;
        }

        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            var checkbox = FindMatchingCheckBox(_owner, item as EventConfigNodeViewModel);
            return new CheckableTreeViewDataItemAutomationPeer(item, this, base.EventsSource as CheckableTreeViewDataItemAutomationPeer, checkbox);
        }
    }
}
