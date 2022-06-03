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
        }

        /// <summary>
        /// Handles click on select all checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbxSelectAll_Click(object sender, RoutedEventArgs e)
        {
            // CheckAllBoxes(lvResults, (sender as CheckBox).IsChecked.Value);
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
            //UIElement uie = e.OriginalSource as UIElement;

            //if ((e.Key == Key.Right || e.Key == Key.Left) && Keyboard.FocusedElement is ListViewItem)
            //{
            //    if (e.Key == Key.Right)
            //    {
            //        var vb = GetFirstChildElement<CheckBox>(sender as DependencyObject) as CheckBox;
            //        vb.Focus();
            //    }
            //    else
            //    {
            //        var parent = GetParentElem<Expander>(sender as DependencyObject) as Expander;
            //        var vb = GetFirstChildElement<Label>(parent as DependencyObject) as Label;
            //        vb.Focus();
            //    }
            //    e.Handled = true;
            //}
            //else if ((e.Key == Key.Right || e.Key == Key.Left) && (Keyboard.FocusedElement is CheckBox || Keyboard.FocusedElement is Button))
            //{
            //    var elements = new List<DependencyObject>();
            //    elements.Add(GetFirstChildElement<CheckBox>(sender as DependencyObject));
            //    elements.AddRange(FindChildren<Button>(sender as DependencyObject));
            //    int selectedElementIndex = elements.FindIndex(b => b.Equals(Keyboard.FocusedElement));

            //    if (e.Key == Key.Right)
            //    {
            //        if (selectedElementIndex + 1 < elements.Count)
            //        {
            //            ((UIElement)elements.ElementAt(selectedElementIndex + 1)).Focus();
            //        }
            //    }
            //    else if (selectedElementIndex - 1 >= 0)
            //    {
            //        ((UIElement)elements.ElementAt(selectedElementIndex - 1)).Focus();
            //    }
            //    else
            //    {
            //        (sender as ListBoxItem).Focus();
            //    }
            //    System.Diagnostics.Debug.WriteLine(Keyboard.FocusedElement.ToString() + " " + FrameworkElementAutomationPeer.FromElement(Keyboard.FocusedElement as FrameworkElement)?.HasKeyboardFocus());
            //    FrameworkElementAutomationPeer.FromElement(Keyboard.FocusedElement as FrameworkElement)?.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
            //    e.Handled = true;
            //}
            //else if (e.Key == Key.Down || e.Key == Key.Up)
            //{
            //    (sender as ListViewItem).Focus();
            //    uie = (UIElement)Keyboard.FocusedElement;

            //    if (e.Key == Key.Down)
            //    {
            //        uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
            //    }
            //    else
            //    {
            //        var element = uie.PredictFocus(FocusNavigationDirection.Up);
            //        if (element is ListViewItem)
            //        {
            //            uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
            //        }
            //        else
            //        {
            //            (GetParentElem<GroupItem>(sender as DependencyObject) as UIElement).Focus();
            //        }
            //    }
            //    e.Handled = true;
            //}
            //else if (e.Key == Key.Return && Keyboard.FocusedElement is ListViewItem)
            //{
            //    var btn = GetFirstChildElement<Button>(sender as DependencyObject) as Button;
            //    ButtonElem_Click(btn, e);
            //}
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
            //var cb = sender as CheckBox;
            //if (cb.IsEnabled)
            //{
            //    var exp = GetParentElem<Expander>(cb) as Expander;
            //    var lst = cb.DataContext as CollectionViewGroup;
            //    var itmsSelected = SetItemsChecked(lst.Items, cb.IsChecked.Value);
            //    if (!itmsSelected)
            //    {
            //        exp.SizeChanged += Exp_Checked;
            //    }

            //    // update tag for whether the group item has children highlighted or not
            //    var groupitem = GetParentElem<GroupItem>(exp) as GroupItem;
            //    groupitem.Tag = cb.IsChecked.Value ? "all" : "zero";
            //}
        }

        /// <summary>
        /// Custom keyboard behavior for group items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //var listViewItemParent = GetParentElem<ListViewItem>(Keyboard.FocusedElement as DependencyObject);
            //if (Keyboard.FocusedElement is ListViewItem || listViewItemParent != null)
            //{
            //    // Let it be handled by the listviewitem previewkeydown handler
            //    //  - this groupitem_previewkeydown event fires first
            //    return;
            //}

            //var gi = sender as GroupItem;
            //var sp = GetFirstChildElement<StackPanel>(gi) as StackPanel;
            //var exp = GetParentElem<Expander>(sp) as Expander;

            //if (e.Key == Key.Right)
            //{
            //    if (!exp.IsExpanded)
            //    {
            //        exp.IsExpanded = true;
            //    }
            //    e.Handled = true;
            //}
            //else if (e.Key == Key.Left)
            //{
            //    if (exp.IsExpanded)
            //    {
            //        exp.IsExpanded = false;
            //    }

            //    e.Handled = true;
            //}
            //else if ((e.Key == Key.Space || e.Key == Key.Enter) && Keyboard.FocusedElement == sender)
            //{
            //    var cb = GetFirstChildElement<CheckBox>(exp) as CheckBox;
            //    cb.IsChecked = !cb.IsChecked ?? false;
            //    CheckBox_Click(cb, null);
            //    e.Handled = true;
            //}
            //else if (e.Key == Key.Down)
            //{
            //    if (!exp.IsExpanded)
            //    {
            //        gi.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
            //    }
            //    else
            //    {
            //        ListViewItem firstResult = GetFirstChildElement<ListViewItem>(exp) as ListViewItem;
            //        firstResult.Focus();
            //    }
            //    e.Handled = true;
            //}
            //else if (e.Key == Key.Up)
            //{
            //    gi.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
            //    e.Handled = true;
            //}
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
