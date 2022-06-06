// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.Extensions.Helpers;
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.SharedUx.ViewModels;
using Axe.Windows.Actions.Contexts;
using Axe.Windows.Core.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    /// <summary>
    /// Interaction logic for AutomatedChecksCustomListControl.xaml
    /// </summary>
    public partial class AutomatedChecksCustomListControl : UserControl
    {
        /// <summary>
        /// This width will limit the element path column to approx. 75 chars
        /// </summary>
        const int MaxElemPathColWidth = 360;

        private bool _allExpanded;

        public ListView ListView => this.content;
        public CheckBox CheckBoxSelectAll => this.chbxSelectAll;

        internal AutomatedChecksCustomListViewModel ViewModel { get; set; }

        internal IEnumerable<RuleResultViewModel> GetSelectedItems()
        {
            if (ViewModel == null) return Enumerable.Empty<RuleResultViewModel>();

            return ViewModel.SelectedItems;
        }

        public AutomatedChecksCustomListControl()
        {
            InitializeComponent();
            AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(Thumb_DragDelta), true);
        }

        internal void Reset()
        {
            SetAllExpanded(false);
            CheckBoxSelectAll.IsChecked = false;
            ViewModel?.SelectedItems?.Clear();
            ListView.ItemsSource = null;
        }

        private void SetAllExpanded(bool allExpanded)
        {
            _allExpanded = allExpanded;
            fabicnExpandAll.GlyphName = _allExpanded ? FabricIcon.CaretSolidDown : FabricIcon.CaretSolidRight;
        }

        /// <summary>
        /// Resize column widths
        /// </summary>
        void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = e.OriginalSource as Thumb;
            var header = thumb.TemplatedParent as GridViewColumnHeader;

            if (header != null && header.Content != null)
            {
                if (header.Column.ActualWidth + e.HorizontalChange < header.MinWidth)
                {
                    header.Column.Width = header.MinWidth;
                }
                else if (header.Column.ActualWidth + e.HorizontalChange > header.MaxWidth)
                {
                    header.Column.Width = header.MaxWidth;
                }
                else
                {
                    header.Column.Width = header.Column.ActualWidth + e.HorizontalChange;
                }
            }
        }

        internal void CheckAllBoxes()
        {
            ViewModel.CheckAllBoxes(content, true);
        }

        /// <summary>
        /// Handles click on select all checkbox
        /// </summary>
        private void chbxSelectAll_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CheckAllBoxes(content, (sender as CheckBox).IsChecked.Value);
        }

        /// <summary>
        /// Handles expand all button click
        /// </summary>
        private void btnExpandAll_Click(object sender, RoutedEventArgs e)
        {
            SetAllExpanded(!_allExpanded);
            ExpandAllExpanders(content);
        }

        /// <summary>
        /// Finds and expands all expanders recursively
        /// </summary>
        public void ExpandAllExpanders(DependencyObject root)
        {
            if (root == null)
            {
                return;
            }

            if (root is Expander)
            {
                (root as Expander).IsExpanded = _allExpanded;
            }

            for (int x = 0; x < VisualTreeHelper.GetChildrenCount(root); x++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(root, x);
                ExpandAllExpanders(child);
            }
        }

        /// <summary>
        /// Handles click on element snippet
        /// </summary>
        private void ButtonElem_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NotifySelected(((Button)sender).Tag as A11yElement);
        }

        /// <summary>
        /// Handles click on file bug button
        /// </summary>
        private void btnFileBug_Click(object sender, RoutedEventArgs e)
        {
            var vm = ((Button)sender).Tag as RuleResultViewModel;
            ViewModel.FileBug(vm);

        }

        private void btnFileBug_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && (HelperMethods.FileBugVisibility == Visibility.Visible))
            {
                ((Button)sender).GetBindingExpression(Button.ContentProperty).UpdateTarget();
            }
        }

        /// <summary>
        /// Handles click on source link button
        /// </summary>
        private void btnSourceLink_Click(object sender, RoutedEventArgs e)
        {
            var vm = ((Button)sender).Tag as RuleResultViewModel;
            AutomatedChecksCustomListViewModel.OpenLinkToRuleSource(vm);
        }

        /// <summary>
        /// Custom keyboard nav behavior
        /// </summary>
        private void lviResults_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ViewModel.OnListViewItemPreviewKeyDown(sender, e);
        }

        /// <summary>
        /// Handles unselecting a listviewitem
        /// </summary>
        private void ListViewItem_Unselected(object sender, RoutedEventArgs e)
        {
            ViewModel.OnListViewItemUnselected(sender);
        }

        /// <summary>
        /// Handles list view item selection
        /// </summary>
        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            ViewModel.OnListViewItemUnselected(sender);
        }

        /// <summary>
        /// Handles group level checkbox click
        /// </summary>
        internal void CheckBoxClick(object sender, RoutedEventArgs e)
        {
            ViewModel.CheckBoxClick(sender, e);
        }

        /// <summary>
        /// Custom keyboard behavior for group items
        /// </summary>
        private void GroupItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ViewModel.OnGroupItemPreviewKeyDown(sender, e);
        }

        /// <summary>
        /// Don't let column auto-size past ~75 characters
        /// </summary>
        private void CustomGridViewColumnHeader_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > MaxElemPathColWidth && Double.IsNaN(gvcElement.Width))
            {
                gvcElement.Width = MaxElemPathColWidth;
            }
        }

        /// <summary>
        /// We disable this because otherwise the scrollviewer will scroll past the
        /// left-most checkboxes and instead have the expander on the very left. This looks
        /// visually jarring.
        /// </summary>
        private void Expander_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Handles expander collapse event
        /// </summary>
        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            SetAllExpanded(false);
        }
    }
}
