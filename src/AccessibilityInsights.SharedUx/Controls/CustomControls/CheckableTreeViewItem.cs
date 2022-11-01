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

        private static T FindDescendant<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            var childCount = VisualTreeHelper.GetChildrenCount(parent);
            if (childCount < 1) return null;

            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                    return child as T;

                var descendant = FindDescendant<T>(child);
                if (descendant != null)
                    return descendant;
            }

            return null;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            var checkbox = FindDescendant<CheckBox>(this);
            if (checkbox == null) return base.OnCreateAutomationPeer();

            return new CheckableTreeViewItemAutomationPeer(this, checkbox);
        }
    }
}
