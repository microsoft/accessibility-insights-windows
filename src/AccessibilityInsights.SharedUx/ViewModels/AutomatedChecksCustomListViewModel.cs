// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Extensions.Helpers;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.Controls.CustomControls;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.FileIssue;
using AccessibilityInsights.SharedUx.Highlighting;
using AccessibilityInsights.SharedUx.Telemetry;
using Axe.Windows.Actions.Contexts;
using Axe.Windows.Core.Bases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    class AutomatedChecksCustomListViewModel : ViewModelBase
    {
        /// <summary>
        /// Whether there is a screenshot available (if not, checkboxes should be disabled)
        /// </summary>
        private bool ScreenshotAvailable => _dataContext != null && _dataContext.Screenshot != null;

        private readonly ElementContext _elementContext;
        private readonly ElementDataContext _dataContext;
        private readonly Action _notifyElementSelected;
        private readonly Action _switchToServerLogin;
        private readonly AutomatedChecksCustomListControl _customListControl;

        /// <summary>
        /// Currently selected items
        /// </summary>
        internal IList<RuleResultViewModel> SelectedItems { get; }


        internal AutomatedChecksCustomListViewModel(AutomatedChecksCustomListControl customListControl, ElementContext elementContext, Action notifyElementSelected,
            Action switchToServerLogin)
        {
            _elementContext = elementContext ?? throw new ArgumentNullException(nameof(elementContext));
            _dataContext = elementContext.DataContext ?? throw new ArgumentException("Null DataContext not allowed", nameof(elementContext));
            _customListControl = customListControl ?? throw new ArgumentNullException(nameof(customListControl));
            _notifyElementSelected = notifyElementSelected ?? throw new ArgumentNullException(nameof(notifyElementSelected));
            _switchToServerLogin = switchToServerLogin ?? throw new ArgumentNullException(nameof(switchToServerLogin));
            SelectedItems = new List<RuleResultViewModel>();

            _customListControl.CheckBoxSelectAll.IsEnabled = ScreenshotAvailable;
            _customListControl.ViewModel = this;
        }

        public void NotifySelected(A11yElement element)
        {
            _dataContext.FocusedElementUniqueId = element.UniqueId;
            _notifyElementSelected();
        }

        /// <summary>
        /// Starts bug fileing for the element identified by the RuleResultViewModel
        /// </summary>
        public void FileBug(RuleResultViewModel vm)
        {
            if (vm.IssueLink != null)
            {
                // Bug already filed, open it in a new window
                try
                {
                    Process.Start(vm.IssueLink.OriginalString);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
                {
                    ex.ReportException();
                    // Happens when bug is deleted, message describes that work item doesn't exist / possible permission issue
                    MessageDialog.Show(ex.InnerException?.Message);
                    vm.IssueDisplayText = null;
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
            else
            {
                // File a new bug
                var telemetryEvent = TelemetryEventFactory.ForIssueFilingRequest(FileBugRequestSource.AutomatedChecks);
                Logger.PublishTelemetryEvent(telemetryEvent);

                if (IssueReporter.IsConnected)
                {
                    IssueInformation issueInformation = vm.GetIssueInformation();
                    FileIssueAction.AttachIssueData(issueInformation, _elementContext.Id, vm.Element.BoundingRectangle, vm.Element.UniqueId);

                    IIssueResult issueResult = FileIssueAction.FileIssueAsync(issueInformation);
                    if (issueResult != null)
                    {
                        vm.IssueDisplayText = issueResult.DisplayText;
                        vm.IssueLink = issueResult.IssueLink;
                    }
                    File.Delete(issueInformation.TestFileName);
                }
                else
                {
                    bool? accepted = MessageDialog.Show(Properties.Resources.AutomatedChecksControl_btnFileBug_Click_File_Issue_Configure);
                    if (accepted.HasValue && accepted.Value)
                    {
                        _switchToServerLogin();
                    }
                }
            }
        }

        /// <summary>
        /// Handles click on source link button
        /// </summary>
        public static void OpenLinkToRuleSource(RuleResultViewModel vm)
        {
            string urlToLaunch = vm?.URL?.ToString();
            if (!string.IsNullOrEmpty(urlToLaunch))
            {
                Process.Start(urlToLaunch);
            }
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
                CheckBoxClick(cb, null);
            }

            for (int x = 0; x < VisualTreeHelper.GetChildrenCount(root); x++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(root, x);
                CheckAllBoxes(child, check);
            }
        }

        /// <summary>
        /// Handles group level checkbox click
        /// </summary>
        internal void CheckBoxClick(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            Expander expander = CheckAllChildren(cb);

            if (expander != null)
            {
                expander.SizeChanged += Exp_Checked;
            }
        }

        /// <summary>
        /// Select expander's elements when expanded
        /// </summary>
        private void Exp_Checked(object sender, SizeChangedEventArgs e)
        {
            var expander = sender as Expander;
            Exp_Checked(expander);
            expander.SizeChanged -= Exp_Checked;
        }

        /// <summary>
        /// Select expander's elements when expanded
        /// </summary>
        private void Exp_Checked(Expander exp)
        {
            var lst = exp.DataContext as CollectionViewGroup;
            var cb = GetFirstChildElement<CheckBox>(exp) as CheckBox;
            SetItemsChecked(lst.Items, cb.IsChecked.Value);
        }

        /// <summary>
        /// Select all items in list
        /// </summary>
        private bool SetItemsChecked(IReadOnlyCollection<Object> lst, bool check)
        {
            var ret = true;
            foreach (RuleResultViewModel itm in lst.AsParallel())
            {
                if (check && !SelectedItems.Contains(itm))
                {
                    SelectedItems.Add(itm);
                    ImageOverlayDriver.GetDefaultInstance().AddElement(_elementContext.Id, itm.Element.UniqueId);
                }

                else if (!check && SelectedItems.Contains(itm))
                {
                    SelectedItems.Remove(itm);
                    ImageOverlayDriver.GetDefaultInstance().RemoveElement(_elementContext.Id, itm.Element.UniqueId);
                }
                var lvi = _customListControl.ListView.ItemContainerGenerator.ContainerFromItem(itm) as ListViewItem;
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
        /// Update select checkbox state
        /// </summary>
        private void UpdateSelectAll()
        {
            if (SelectedItems.Count == 0)
            {
                _customListControl.CheckBoxSelectAll.IsChecked = false;
            }
            else if (SelectedItems.Count == _customListControl.ListView.Items.Count)
            {
                _customListControl.CheckBoxSelectAll.IsChecked = true;
            }
            else
            {
                _customListControl.CheckBoxSelectAll.IsChecked = null;
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

        public Expander CheckAllChildren(CheckBox cb)
        {
            Expander expander = null;

            if (cb.IsEnabled)
            {
                var exp = GetParentElem<Expander>(cb) as Expander;
                var lst = cb.DataContext as CollectionViewGroup;
                if (SetItemsChecked(lst.Items, cb.IsChecked.Value))
                {
                    expander = exp;
                }

                // update tag for whether the group item has children highlighted or not
                var groupitem = GetParentElem<GroupItem>(exp) as GroupItem;
                groupitem.Tag = cb.IsChecked.Value ? "all" : "zero";
            }

            return expander;
        }

        /// <summary>
        /// Finds object up parent hierarchy of specified type
        /// </summary>
        private DependencyObject GetParentElem<T>(DependencyObject obj)
        {
            try
            {
                var par = VisualTreeHelper.GetParent(obj);

                if (par is T)
                {
                    return par;
                }
                else
                {
                    return GetParentElem<T>(par);
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                e.ReportException();
                return null;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Custom keyboard nav behavior
        /// </summary>
        public void OnListViewItemPreviewKeyDown(object sender, KeyEventArgs e)
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
                    var parent = GetParentElem<Expander>(sender as DependencyObject) as Expander;
                    var vb = GetFirstChildElement<Label>(parent as DependencyObject) as Label;
                    vb.Focus();
                }
                e.Handled = true;
            }
            else if ((e.Key == Key.Right || e.Key == Key.Left) && (Keyboard.FocusedElement is CheckBox || Keyboard.FocusedElement is Button))
            {
                var elements = new List<DependencyObject>();
                elements.Add(GetFirstChildElement<CheckBox>(sender as DependencyObject));
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
                        (GetParentElem<GroupItem>(sender as DependencyObject) as UIElement).Focus();
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
        /// Custom keyboard behavior for group items
        /// </summary>
        public void OnGroupItemPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var listViewItemParent = GetParentElem<ListViewItem>(Keyboard.FocusedElement as DependencyObject);
            if (Keyboard.FocusedElement is ListViewItem || listViewItemParent != null)
            {
                // Let it be handled by the listviewitem previewkeydown handler
                //  - this groupitem_previewkeydown event fires first
                return;
            }

            var gi = sender as GroupItem;
            var sp = GetFirstChildElement<StackPanel>(gi) as StackPanel;
            var exp = GetParentElem<Expander>(sp) as Expander;

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
                CheckBoxClick(cb, null);
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
        /// Handles unselecting a listviewitem
        /// </summary>
        internal void OnListViewItemUnselected(object sender)
        {
            var lvi = sender as ListViewItem;
            var exp = GetParentElem<Expander>(lvi) as Expander;
            var cb = GetFirstChildElement<CheckBox>(exp) as CheckBox;
            var srvm = lvi.DataContext as RuleResultViewModel;

            // ElementContext can be null when app is closed.
            if (this._elementContext != null)
            {
                ImageOverlayDriver.GetDefaultInstance().RemoveElement(_elementContext.Id, srvm.Element.UniqueId);
            }

            SelectedItems.Remove(srvm);
            var groupitem = GetParentElem<GroupItem>(exp) as GroupItem;
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
        /// Handles list view item selection
        /// </summary>
        internal void OnListViewItemSelected(object sender)
        {
            if (ScreenshotAvailable)
            {
                var lvi = sender as ListViewItem;
                var exp = GetParentElem<Expander>(lvi) as Expander;
                var cb = GetFirstChildElement<CheckBox>(exp) as CheckBox;
                var itm = lvi.DataContext as RuleResultViewModel;
                if (!SelectedItems.Contains(itm))
                {
                    SelectedItems.Add(itm);
                    UpdateSelectAll();
                    ImageOverlayDriver.GetDefaultInstance().AddElement(_elementContext.Id, itm.Element.UniqueId);
                }
                var groupitem = GetParentElem<GroupItem>(exp) as GroupItem;
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
    }
}
