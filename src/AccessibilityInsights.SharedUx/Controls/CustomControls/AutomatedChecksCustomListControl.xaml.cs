// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.Extensions.Helpers;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Highlighting;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.SharedUx.ViewModels;
using Axe.Windows.Core.Bases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

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

        private CheckBox CheckBoxSelectAll => this.chbxSelectAll;

        /// <summary>
        /// Whether there is a screenshot available (if not, checkboxes should be disabled)
        /// </summary>
        private bool ScreenshotAvailable => _controlContext?.DataContext?.Screenshot != null;

        private AutomatedChecksCustomListControlContext _controlContext;

        #region DataGridAccessibleName (Dependency Property)

        public string DataGridAccessibleName
        {
            get { return (string)GetValue(DataGridAccessibleNameProperty); }
            set { SetValue(DataGridAccessibleNameProperty, value); }
        }

        public static readonly DependencyProperty DataGridAccessibleNameProperty =
            DependencyProperty.Register("DataGridAccessibleName", typeof(string), typeof(AutomatedChecksCustomListControl), new PropertyMetadata(null));

        #endregion

        #region DataGridAutomationId (Dependency Property)

        public string DataGridAutomationId
        {
            get { return (string)GetValue(DataGridAutomationIdProperty); }
            set { SetValue(DataGridAutomationIdProperty, value); }
        }

        public static readonly DependencyProperty DataGridAutomationIdProperty =
            DependencyProperty.Register("DataGridAutomationId", typeof(string), typeof(AutomatedChecksCustomListControl), new PropertyMetadata(null));

        #endregion

        #region DataGridExpandAllAutomationId (Dependency Property)

        public string DataGridExpandAllAutomationId
        {
            get { return (string)GetValue(DataGridExpandAllAutomationIdProperty); }
            set { SetValue(DataGridExpandAllAutomationIdProperty, value); }
        }

        public static readonly DependencyProperty DataGridExpandAllAutomationIdProperty =
            DependencyProperty.Register("DataGridExpandAllAutomationId", typeof(string), typeof(AutomatedChecksCustomListControl), new PropertyMetadata(null, OnDataGridExpandAllAutomationIdChanged));

        public static void OnDataGridExpandAllAutomationIdChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AutomatedChecksCustomListControl sender = o as AutomatedChecksCustomListControl;
            sender?.btnExpandAll.SetValue(AutomationProperties.AutomationIdProperty, sender.DataGridExpandAllAutomationId);
        }

        #endregion

        #region SectionHeader (Dependency Property)

        public string SectionHeader
        {
            get { return (string)GetValue(SectionHeaderProperty); }
            set { SetValue(SectionHeaderProperty, value); }
        }

        public static readonly DependencyProperty SectionHeaderProperty =
            DependencyProperty.Register("SectionHeader", typeof(string), typeof(AutomatedChecksCustomListControl), new PropertyMetadata(null));

        #endregion

        /// <summary>
        /// Currently selected items
        /// </summary>
        internal IList<RuleResultViewModel> SelectedItems { get; } = new List<RuleResultViewModel>();

        public AutomatedChecksCustomListControl()
        {
            InitializeComponent();
            AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(Thumb_DragDelta), true);
        }

        internal void SetControlContext(AutomatedChecksCustomListControlContext controlContext)
        {
            _controlContext = controlContext ?? throw new ArgumentNullException(nameof(controlContext));

            CheckBoxSelectAll.IsEnabled = ScreenshotAvailable;
            sectionHeader.Visibility = string.IsNullOrEmpty(SectionHeader) ? Visibility.Collapsed : Visibility.Visible;
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
        /// Handles click on element snippet
        /// </summary>
        private void ButtonElem_Click(object sender, RoutedEventArgs e)
        {
            NotifySelected(((Button)sender).Tag as A11yElement);
        }

        /// <summary>
        /// Notify Element selection to switch mode to snapshot
        /// </summary>
        private void NotifySelected(A11yElement element)
        {
            if (element != null)
            {
                _controlContext.DataContext.FocusedElementUniqueId = element.UniqueId;
                _controlContext.NotifyElementSelected();
            }
        }

        /// <summary>
        /// Handles click on source link button
        /// </summary>
        private void btnSourceLink_Click(object sender, RoutedEventArgs e)
        {
            var vm = ((Button)sender).Tag as RuleResultViewModel;
            string urlToLaunch = vm?.URL?.ToString();
            if (!string.IsNullOrEmpty(urlToLaunch))
            {
                Process.Start(urlToLaunch);
            }
        }

        /// <summary>
        /// Handles expand all button click
        /// </summary>
        private void btnExpandAll_Click(object sender, RoutedEventArgs e)
        {
            SetAllExpanded(!_allExpanded);
            ExpandAllExpanders(lvResults);
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
        /// Handles expander collapse event
        /// </summary>
        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            SetAllExpanded(false);
        }


        /// <summary>
        /// Finds all controls of the given type under the given object
        /// </summary>
        public IEnumerable<T> FindChildren<T>(DependencyObject element) where T : DependencyObject
        {
            if (element != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(element, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }
                    foreach (T descendant in FindChildren<T>(child))
                    {
                        yield return descendant;
                    }
                }
            }
        }

        /// <summary>
        /// Get child element of specified type
        /// </summary>
        private DependencyObject GetFirstChildElement<T>(DependencyObject element)
        {
            if (element == null)
            {
                return null;
            }

            if (element is T)
            {
                return element as DependencyObject;
            }

            for (int x = 0; x < VisualTreeHelper.GetChildrenCount(element); x++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element, x);
                var b = GetFirstChildElement<T>(child);

                if (b != null)
                    return b;
            }
            return null;
        }

        /// <summary>
        /// Custom keyboard nav behavior
        /// </summary>
        private void lviResults_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            UIElement uie = e.OriginalSource as UIElement;

            if ((e.Key == Key.Right || e.Key == Key.Left) && Keyboard.FocusedElement is ListViewItem)
            {
                if (e.Key == Key.Right)
                {
                    var vb = GetFirstChildElement<CheckBox>(sender as DependencyObject) as CheckBox;
                    vb.Focus();
                }
                else
                {
                    var parent = (sender as DependencyObject).GetParentElem<Expander>() as Expander;
                    var vb = GetFirstChildElement<Label>(parent as DependencyObject) as Label;
                    vb.Focus();
                }
                e.Handled = true;
            }
            else if ((e.Key == Key.Right || e.Key == Key.Left) && (Keyboard.FocusedElement is CheckBox || Keyboard.FocusedElement is Button))
            {
                var elements = new List<DependencyObject>
                {
                    GetFirstChildElement<CheckBox>(sender as DependencyObject)
                };
                elements.AddRange(FindChildren<Button>(sender as DependencyObject));
                int selectedElementIndex = elements.FindIndex(b => b.Equals(Keyboard.FocusedElement));

                if (e.Key == Key.Right)
                {
                    if (selectedElementIndex + 1 < elements.Count)
                    {
                        ((UIElement)elements.ElementAt(selectedElementIndex + 1)).Focus();
                    }
                }
                else if (selectedElementIndex - 1 >= 0)
                {
                    ((UIElement)elements.ElementAt(selectedElementIndex - 1)).Focus();
                }
                else
                {
                    (sender as ListBoxItem).Focus();
                }
                System.Diagnostics.Debug.WriteLine(Keyboard.FocusedElement.ToString() + " " + FrameworkElementAutomationPeer.FromElement(Keyboard.FocusedElement as FrameworkElement)?.HasKeyboardFocus());
                FrameworkElementAutomationPeer.FromElement(Keyboard.FocusedElement as FrameworkElement)?.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
                e.Handled = true;
            }
            else if (e.Key == Key.Down || e.Key == Key.Up)
            {
                (sender as ListViewItem).Focus();
                uie = (UIElement)Keyboard.FocusedElement;

                if (e.Key == Key.Down)
                {
                    uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                }
                else
                {
                    var element = uie.PredictFocus(FocusNavigationDirection.Up);
                    if (element is ListViewItem)
                    {
                        uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
                    }
                    else
                    {
                        ((sender as DependencyObject).GetParentElem<GroupItem>() as UIElement).Focus();
                    }
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Return && Keyboard.FocusedElement is ListViewItem)
            {
                var btn = GetFirstChildElement<Button>(sender as DependencyObject) as Button;
                NotifySelected(btn.Tag as A11yElement);
            }
        }

        internal void CheckAllBoxes()
        {
            CheckAllBoxes(lvResults, true);
        }

        /// <summary>
        /// Finds and expands all expanders recursively
        /// </summary>
        internal void CheckAllBoxes(DependencyObject root, bool check)
        {
            if (root == null)
            {
                return;
            }

            if (root is CheckBox cb && cb.Tag != null)
            {
                cb.IsChecked = check;
                CheckBox_Click(cb, null);
            }

            for (int x = 0; x < VisualTreeHelper.GetChildrenCount(root); x++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(root, x);
                CheckAllBoxes(child, check);
            }
        }

        /// <summary>
        /// Select all items in list
        /// </summary>
        private bool SetItemsChecked(IReadOnlyCollection<Object> lst, bool check)
        {
            var ret = true;
            foreach (RuleResultViewModel itm in lst.AsParallel().Cast<RuleResultViewModel>())
            {
                if (check && !SelectedItems.Contains(itm))
                {
                    SelectedItems.Add(itm);
                    ImageOverlayDriver.GetDefaultInstance().AddElement(_controlContext.ElementContext.Id, itm.Element.UniqueId);
                }

                else if (!check && SelectedItems.Contains(itm))
                {
                    SelectedItems.Remove(itm);
                    ImageOverlayDriver.GetDefaultInstance().RemoveElement(_controlContext.ElementContext.Id, itm.Element.UniqueId);
                }
                var lvi = lvResults.ItemContainerGenerator.ContainerFromItem(itm) as ListViewItem;
                if (lvi != null)
                {
                    lvi.IsSelected = check;
                }
                else
                {
                    ret = false;
                }
            }
            UpdateSelectAll();
            return ret;
        }

        /// <summary>
        /// Select expander's elements when expanded
        /// </summary>
        private void Exp_Checked(object sender, SizeChangedEventArgs e)
        {
            var exp = sender as Expander;
            var lst = exp.DataContext as CollectionViewGroup;
            var cb = GetFirstChildElement<CheckBox>(exp) as CheckBox;
            SetItemsChecked(lst.Items, cb.IsChecked.Value);
            exp.SizeChanged -= Exp_Checked;
        }

        /// <summary>
        /// Handles unselecting a listviewitem
        /// </summary>
        private void ListViewItem_Unselected(object sender, RoutedEventArgs e)
        {
            var lvi = sender as ListViewItem;
            var exp = lvi.GetParentElem<Expander>() as Expander;
            var cb = GetFirstChildElement<CheckBox>(exp) as CheckBox;
            var srvm = lvi.DataContext as RuleResultViewModel;

            // ElementContext can be null when app is closed.
            if (this._controlContext.ElementContext != null)
            {
                ImageOverlayDriver.GetDefaultInstance().RemoveElement(_controlContext.ElementContext.Id, srvm.Element.UniqueId);
            }

            SelectedItems.Remove(srvm);
            var groupitem = exp.GetParentElem<GroupItem>() as GroupItem;
            var dc = cb.DataContext as CollectionViewGroup;
            var itms = dc.Items;
            var any = itms.Intersect(SelectedItems).Any();
            if (any)
            {
                cb.IsChecked = null;
                groupitem.Tag = "some";
            }
            else
            {
                cb.IsChecked = false;
                groupitem.Tag = "zero";
            }
            UpdateSelectAll();
        }

        /// <summary>
        /// Update select checkbox state
        /// </summary>
        private void UpdateSelectAll()
        {
            if (SelectedItems.Count == 0)
            {
                CheckBoxSelectAll.IsChecked = false;
            }
            else if (SelectedItems.Count == lvResults.Items.Count)
            {
                CheckBoxSelectAll.IsChecked = true;
            }
            else
            {
                CheckBoxSelectAll.IsChecked = null;
            }
        }

        /// <summary>
        /// Handles group level checkbox click
        /// </summary>
        internal void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb.IsEnabled)
            {
                var exp = cb.GetParentElem<Expander>() as Expander;
                var lst = cb.DataContext as CollectionViewGroup;
                var itemsSelected = SetItemsChecked(lst.Items, cb.IsChecked.Value);
                if (!itemsSelected)
                {
                    exp.SizeChanged += Exp_Checked;
                }

                // update tag for whether the group item has children highlighted or not
                var groupitem = exp.GetParentElem<GroupItem>() as GroupItem;
                groupitem.Tag = cb.IsChecked.Value ? "all" : "zero";
            }
        }

        /// <summary>
        /// Handles click on select all checkbox
        /// </summary>
        private void chbxSelectAll_Click(object sender, RoutedEventArgs e)
        {
            CheckAllBoxes(lvResults, (sender as CheckBox).IsChecked.Value);
        }

        /// <summary>
        /// Handles list view item selection
        /// </summary>
        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            if (ScreenshotAvailable)
            {
                var lvi = sender as ListViewItem;
                var exp = lvi.GetParentElem<Expander>() as Expander;
                var cb = GetFirstChildElement<CheckBox>(exp) as CheckBox;
                var itm = lvi.DataContext as RuleResultViewModel;
                if (!SelectedItems.Contains(itm))
                {
                    SelectedItems.Add(itm);
                    UpdateSelectAll();
                    ImageOverlayDriver.GetDefaultInstance().AddElement(_controlContext.ElementContext.Id, itm.Element.UniqueId);
                }
                var groupitem = exp.GetParentElem<GroupItem>() as GroupItem;
                var dc = cb.DataContext as CollectionViewGroup;
                var itms = dc.Items;
                var any = itms.Except(SelectedItems).Any();
                if (any)
                {
                    cb.IsChecked = null;
                    groupitem.Tag = "some"; // used to indicate how many children are selected
                }
                else
                {
                    cb.IsChecked = true;
                    groupitem.Tag = "all";
                }
            }
        }

        /// <summary>
        /// Custom keyboard behavior for group items
        /// </summary>
        private void GroupItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var listViewItemParent = (Keyboard.FocusedElement as DependencyObject).GetParentElem<ListViewItem>();
            if (Keyboard.FocusedElement is ListViewItem || listViewItemParent != null)
            {
                // Let it be handled by the listviewitem previewkeydown handler
                //  - this groupitem_previewkeydown event fires first
                return;
            }

            var gi = sender as GroupItem;
            var sp = GetFirstChildElement<StackPanel>(gi) as StackPanel;
            var exp = sp.GetParentElem<Expander>() as Expander;

            if (e.Key == Key.Right)
            {
                if (!exp.IsExpanded)
                {
                    exp.IsExpanded = true;
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Left)
            {
                if (exp.IsExpanded)
                {
                    exp.IsExpanded = false;
                }

                e.Handled = true;
            }
            else if ((e.Key == Key.Space || e.Key == Key.Enter) && Keyboard.FocusedElement == sender)
            {
                var cb = GetFirstChildElement<CheckBox>(exp) as CheckBox;
                cb.IsChecked = !cb.IsChecked ?? false;
                CheckBox_Click(cb, null);
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                if (!exp.IsExpanded)
                {
                    gi.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                }
                else
                {
                    ListViewItem firstResult = GetFirstChildElement<ListViewItem>(exp) as ListViewItem;
                    firstResult.Focus();
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                gi.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
                e.Handled = true;
            }
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
        /// Handles click on file bug button
        /// </summary>
        private void btnFileBug_Click(object sender, RoutedEventArgs e)
        {
            var vm = ((Button)sender).Tag as RuleResultViewModel;
            var input = new FileIssueWrapperInput(
                vm,
                _controlContext.ElementContext.Id,
                _controlContext.SwitchToServerLogin,
                vm.GetIssueInformation,
                FileBugRequestSource.AutomatedChecks);
            FileIssueWrapper.FileIssueFromControl(input);
        }

        private void btnFileBug_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && (HelperMethods.FileBugVisibility == Visibility.Visible))
            {
                ((Button)sender).GetBindingExpression(Button.ContentProperty).UpdateTarget();
            }
        }

        internal void Reset()
        {
            SetAllExpanded(false);
            CheckBoxSelectAll.IsChecked = false;
            SelectedItems.Clear();
            lvResults.ItemsSource = null;
        }

        internal void SetItemsSource(IEnumerable<RuleResultViewModel> results)
        {
            if (results == null)
            {
                lvResults.ItemsSource = null;
                Visibility = Visibility.Collapsed;
            }
            else
            {
                lvResults.IsEnabled = true;
                lvResults.ItemsSource = results;
                Visibility = Visibility.Visible;
            }
        }

        private void SetAllExpanded(bool allExpanded)
        {
            _allExpanded = allExpanded;
            fabicnExpandAll.GlyphName = _allExpanded ? FabricIcon.CaretSolidDown : FabricIcon.CaretSolidRight;
        }

        internal void AddGroupDescription(PropertyGroupDescription groupDescription)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvResults.ItemsSource);
            view.GroupDescriptions.Add(groupDescription);
        }
    }
}
