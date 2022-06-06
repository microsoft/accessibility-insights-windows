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

        public ListView ListView => this.content;
        public FabricIconControl FabricIconExpandAll => this.fabicnExpandAll;
        public CheckBox CheckBoxSelectAll => this.chbxSelectAll;

        internal AutomatedChecksCustomListViewModel ViewModel { get; set; }

        public AutomatedChecksCustomListControl()
        {
            InitializeComponent();
            AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(Thumb_DragDelta), true);
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

        /// <summary>
        /// Handles click on select all checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbxSelectAll_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CheckAllBoxes(content, (sender as CheckBox).IsChecked.Value);
        }

        /// <summary>
        /// Handles expand all button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExpandAll_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.ToggleExpandAll(content))
            {
                fabicnExpandAll.GlyphName = FabricIcon.CaretSolidDown;
            }
            else
            {
                fabicnExpandAll.GlyphName = FabricIcon.CaretSolidRight;
            }
        }

        /// <summary>
        /// Handles click on element snippet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonElem_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NotifySelected(((Button)sender).Tag as A11yElement);
        }

        /// <summary>
        /// Handles click on file bug button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSourceLink_Click(object sender, RoutedEventArgs e)
        {
            var vm = ((Button)sender).Tag as RuleResultViewModel;
            AutomatedChecksCustomListViewModel.OpenLinkToRuleSource(vm);
        }

        /// <summary>
        /// Custom keyboard nav behavior
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lviResults_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ViewModel.OnListViewItemPreviewKeyDown(sender, e);
        }

        /// <summary>
        /// Handles unselecting a listviewitem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewItem_Unselected(object sender, RoutedEventArgs e)
        {
            //var lvi = sender as ListViewItem;
            //var exp = GetParentElem<Expander>(lvi) as Expander;
            //var cb = GetFirstChildElement<CheckBox>(exp) as CheckBox;
            //var srvm = lvi.DataContext as RuleResultViewModel;

            //// ElementContext can be null when app is closed.
            //if (this.ElementContext != null)
            //{
            //    ImageOverlayDriver.GetDefaultInstance().RemoveElement(this.ElementContext.Id, srvm.Element.UniqueId);
            //}

            //SelectedItems.Remove(srvm);
            //var groupitem = GetParentElem<GroupItem>(exp) as GroupItem;
            //var dc = cb.DataContext as CollectionViewGroup;
            //var itms = dc.Items;
            //var any = itms.Intersect(SelectedItems).Any();
            //if (any)
            //{
            //    cb.IsChecked = null;
            //    groupitem.Tag = "some";
            //}
            //else
            //{
            //    cb.IsChecked = false;
            //    groupitem.Tag = "zero";
            //}
            //UpdateSelectAll();
        }

        /// <summary>
        /// Handles list view item selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            //if (ScreenshotAvailable)
            //{
            //    var lvi = sender as ListViewItem;
            //    var exp = GetParentElem<Expander>(lvi) as Expander;
            //    var cb = GetFirstChildElement<CheckBox>(exp) as CheckBox;
            //    var itm = lvi.DataContext as RuleResultViewModel;
            //    if (!SelectedItems.Contains(itm))
            //    {
            //        SelectedItems.Add(itm);
            //        UpdateSelectAll();
            //        ImageOverlayDriver.GetDefaultInstance().AddElement(this.ElementContext.Id, itm.Element.UniqueId);
            //    }
            //    var groupitem = GetParentElem<GroupItem>(exp) as GroupItem;
            //    var dc = cb.DataContext as CollectionViewGroup;
            //    var itms = dc.Items;
            //    var any = itms.Except(SelectedItems).Any();
            //    if (any)
            //    {
            //        cb.IsChecked = null;
            //        groupitem.Tag = "some"; // used to indicate how many children are selected
            //    }
            //    else
            //    {
            //        cb.IsChecked = true;
            //        groupitem.Tag = "all";
            //    }
            //}
        }

        /// <summary>
        /// Handles group level checkbox click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void CheckBoxClick(object sender, RoutedEventArgs e)
        {
            ViewModel.CheckBoxClick(sender, e);
        }

        /// <summary>
        /// Custom keyboard behavior for group items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ViewModel.OnGroupItemPreviewKeyDown(sender, e);
        }

        /// <summary>
        /// Don't let column auto-size past ~75 characters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Expander_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Handles expander collapse event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            ViewModel.AllExpanded = false;
            fabicnExpandAll.GlyphName = FabricIcon.CaretSolidRight;
        }
    }
}
