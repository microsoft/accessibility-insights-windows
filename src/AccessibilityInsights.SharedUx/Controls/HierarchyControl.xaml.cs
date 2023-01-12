// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Interfaces;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.SharedUx.ViewModels;
using Axe.Windows.Actions;
using Axe.Windows.Actions.Contexts;
using Axe.Windows.Actions.Enums;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Misc;
using Axe.Windows.Desktop.UIAutomation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace AccessibilityInsights.SharedUx.Controls
{
    /// <summary>
    /// Interaction logic for HierarchyControl.xaml
    /// </summary>
    public partial class HierarchyControl : UserControl
    {
        /// <summary>
        /// Selected Element in Tree hierarchy
        /// it is used by external code.
        /// </summary>
        public A11yElement SelectedInHierarchyElement { get; private set; }

        /// <summary>
        /// Element that was selected originally to set up this tree hierarchy.
        /// </summary>
        A11yElement _selectedElement;

        /// <summary>
        /// Root Node ViewModel on Hierarchy tree
        /// </summary>
        HierarchyNodeViewModel _rootNode;

        /// <summary>
        /// Contains necessary actions for hierarchy control
        /// </summary>
        public IHierarchyAction HierarchyActions { get; set; }

        /// <summary>
        /// Const values for floating button sizing and spacing
        /// </summary>
        const int TreeButtonHeight = 24;
        const int TreeButtonHorizontalSpacing = 28;
        const int TreeButtonVerticalSpacing = 36;

        /// <summary>
        /// App configuration
        /// </summary>
        public static ConfigurationModel Configuration
        {
            get
            {
                return ConfigurationManager.GetDefaultInstance()?.AppConfig;
            }
        }

        bool _isLiveMode = true;
        /// <summary>
        /// Indicate whether the tree is in Live or Snapshot mode.
        /// </summary>
        public bool IsLiveMode
        {
            get
            {
                return _isLiveMode;
            }

            set
            {
                this._isLiveMode = value;
            }
        }

        /// <summary>
        /// Help text for search box
        /// </summary>
        public string SearchBoxHelpText
        {
            get
            {
                if (string.IsNullOrEmpty(this.textboxSearch.Text))
                {
                    return this.IsLiveMode ? Properties.Resources.SetterValueSearchByName : Properties.Resources.SetterValueSearchByString;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public HierarchyControl()
        {
            InitializeComponent();
            btnMenu.DataContext = this;
        }

        /// <summary>
        /// Overriding LocalizedControlType
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "pane");
        }

        /// <summary>
        /// Set Element to populate UI
        /// </summary>
        /// <param name="e"></param>
        /// <param name="isLiveData">if it is false, it means that data is stale. refresh button should be disabled.</param>
        public void SetElement(ElementContext ec, bool expandall = false)
        {
            if (ec != null)
            {
                this.ElementContext = ec;
                if (ec.Element.PlatformObject != null)
                {
                    PopulateHierarchyTree(ec, expandall);
                }
                else
                {
                    UpdateTreeView(ec.DataContext.GetRootNodeHierarchyViewModel(Configuration.ShowAncestry, Configuration.ShowUncertain, this.IsLiveMode), expandall);
                    this._selectedElement = ec.Element;
                }

                UpdateButtonVisibility();

                // We remove and re-add the "show uncertain" item from the visual tree
                // to ensure n-of-m information is read correctly
                // by screen readers via the SizeOfSet and PositionInSet properties.
                if (cmHierarchySettings.Items.Contains(mniShowUncertain))
                {
                    cmHierarchySettings.Items.Remove(mniShowUncertain);
                }
                if (this.IsLiveMode == false)
                {
                    cmHierarchySettings.Items.Add(mniShowUncertain);
                }
            }
        }

        /// <summary>
        /// Update btnMoveToParent and btnEventMode based on live mode and loaded states
        /// </summary>
        private void UpdateButtonVisibility()
        {
            if (IsLiveMode)
            {
                this.btnMenu.Visibility = Visibility.Visible;
                this.btnTestElement.Visibility = Visibility.Visible;
            }
            else if (this.ElementContext != null && this.ElementContext.DataContext != null && this.ElementContext.DataContext.Mode == DataContextMode.Test)
            {
                this.btnMenu.Visibility = Visibility.Collapsed;
                this.btnTestElement.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.btnMenu.Visibility = Visibility.Collapsed;
                this.btnTestElement.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Event handler for Selected Item changed in Tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeviewHierarchy_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.treeviewHierarchy.SelectedItem != null)
            {
                this.SelectedInHierarchyElement = (A11yElement)((HierarchyNodeViewModel)this.treeviewHierarchy.SelectedItem).Element;

                this.HierarchyActions.SelectedElementChanged();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="runtimeId">RuntimeID of element to select</param>
        public void SelectElement(int uniqueId)
        {
            Select((HierarchyNodeViewModel)treeviewHierarchy.Items[0], uniqueId);
        }

        /// <summary>
        /// Find, select, and expand to element in tree
        /// </summary>
        /// <param name="node"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public bool Select(HierarchyNodeViewModel node, int uniqueId)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            foreach (var child in node.Children)
            {
                if (child.Element.UniqueId == uniqueId)
                {
                    if (child.IsSelected)
                    {
                        this.HierarchyActions.SelectedElementChanged();
                    }
                    else
                    {
                        child.IsSelected = true;
                    }
                    return true;
                }
                else
                {
                    if (Select(child, uniqueId))
                    {
                        child.IsExpanded = true;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Set focus on hierarchy tree
        /// </summary>
        public void SetFocusOnHierarchyTree()
        {
            this.treeviewHierarchy.Focus();
        }

        /// <summary>
        /// Populate Hierarchy tree in live mode
        /// </summary>
        /// <param name="ec"></param>
        /// <param name="expandall"></param>
        private void PopulateHierarchyTree(ElementContext ec, bool expandall)
        {
#if POPULATE_HIERARCHY_TREE_DIAGNOSTICS
            var begin = DateTime.Now;
            var tm = Configuration.TreeViewMode;
            var showa = Configuration.ShowAncestry;
#endif
            /// in the case that UIElement is not alive any more, it will fail.
            /// we need to handle it properly
            HierarchyNodeViewModel rnvm = ec.DataContext.GetRootNodeHierarchyViewModel(Configuration.ShowAncestry, Configuration.ShowUncertain, IsLiveMode);

            // send exception to mode control.
            if (rnvm == null)
            {
                throw new ApplicationException(Properties.Resources.HierarchyControl_PopulateHierarchyTree_No_data_to_populate_hierarchy);
            }

            UpdateTreeView(rnvm, expandall);

            this._selectedElement = ec.Element;
#if POPULATE_HIERARCHY_TREE_DIAGNOSTICS
            var span = DateTime.Now - begin;
#endif
            this.tbTimeSpan.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Update Tree View
        /// </summary>
        /// <param name="rnvm">RootNode View Model</param>
        /// <param name="se">Selected element in hierarchy</param>
        private void UpdateTreeView(HierarchyNodeViewModel rnvm, bool expandall)
        {
            if (!SelectAction.GetDefaultInstance().IsPaused)
            {
                CleanUpTreeView();
            }
            this._rootNode = rnvm;
            this.treeviewHierarchy.ItemsSource = new List<HierarchyNodeViewModel>() { rnvm };
            this.treeviewHierarchy.IsEnabled = true;
            this.HierarchyActions.SelectedElementChanged();

            // expand all if it is required.
            if (expandall)
            {
                rnvm.Expand(true);
            }

            textboxSearch.Text = "";
        }

        private void textboxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this._rootNode != null)
            {
                this._rootNode.ApplyFilter(this.textboxSearch.Text);

                // we do the following so that when focus moves to the tree, focus will be placed on a tree item
                // Otherwise, keyboard users won't be able to navigate through the tree.
                this._rootNode.SelectFirstVisibleLeafNode();

                FireAsyncContentLoadedEvent();
            }
            this.textboxSearch.SetValue(AutomationProperties.HelpTextProperty, SearchBoxHelpText);
        }

        /// <summary>
        /// Notify the tree update when search is complete.
        /// </summary>
        private void FireAsyncContentLoadedEvent()
        {
            if (AutomationPeer.ListenerExists(AutomationEvents.AsyncContentLoaded))
            {
                TreeViewAutomationPeer peer = UIElementAutomationPeer.FromElement(this.treeviewHierarchy) as TreeViewAutomationPeer;
                peer?.RaiseAsyncContentLoadedEvent(new AsyncContentLoadedEventArgs(AsyncContentLoadedState.Completed, 100));
            }
        }

        /// <summary>
        /// Clear Tree
        /// </summary>
        public void Clear()
        {
            if (!SelectAction.GetDefaultInstance().IsPaused)
            {
                CleanUpTreeView();
                this.SelectedInHierarchyElement = null;
                this.ElementContext = null;
                // clean up all data.
                this._selectedElement = null;
                this._rootNode = null;
                this.btnTestElement.Visibility = Visibility.Collapsed;
                this.btnMenu.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Release TreeView ItemsSource
        /// </summary>
        public void CleanUpTreeView()
        {
            if (this.treeviewHierarchy.ItemsSource != null)
            {
                try
                {
                    ((List<HierarchyNodeViewModel>)this.treeviewHierarchy.ItemsSource)[0]?.Clear();
                    this.treeviewHierarchy.ItemsSource = null;
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
                {
                    e.ReportException();
                    // silently ignore.
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

        /// <summary>
        /// Event handler for Move to Parent button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMoveToParent_Click(object sender, RoutedEventArgs e)
        {
            if (this._selectedElement.Parent != null)
            {
                if (this._selectedElement.Parent.IsRootElement() == false)
                {
                    var sa = SelectAction.GetDefaultInstance();
                    sa.SetCandidateElement(this.ElementContext.Id, this._selectedElement.Parent.UniqueId);
                    sa.Select();
                    this.HierarchyActions.RefreshHierarchy(true);
                }
                else
                {
                    MessageDialog.Show(Properties.Resources.HierarchyControl_btnMoveToParent_Click_The_parent_is_Desktop_Element__Desktop_Element_can_t_be_snapshot);
                }
            }
            else
            {
                MessageDialog.Show(Properties.Resources.HierarchyControl_btnMoveToParent_Click_Parent_element_can_t_be_retrieved);
            }
        }

        /// <summary>
        /// GotFocus Event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeviewHierarchy_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.treeviewHierarchy.SelectedItem == null)
            {
                if (this._rootNode != null)
                {
                    this._rootNode.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// Returns currently selected element in hierarchy tree
        /// </summary>
        /// <returns></returns>
        public A11yElement SelectedElement => (this.treeviewHierarchy.SelectedItem as HierarchyNodeViewModel).Element;

        #region Handle context menu for showing ancestry
        /// <summary>
        /// Event handler for Show Ancestry menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniShowAncestry_Click(object sender, RoutedEventArgs e)
        {
            SetFocusOnHierarchyTree();
            Configuration.ShowAncestry = this.mniShowAncestry.IsChecked;
            if (this._selectedElement != null)
            {
                this.HierarchyActions.RefreshHierarchy(false);
            }
        }

        /// <summary>
        /// reflect the status of Ancestry setting.
        /// since data binding is not working. I'm using this way.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniShowAncestry_Loaded(object sender, RoutedEventArgs e)
        {
            ((MenuItem)sender).IsChecked = Configuration.ShowAncestry;
        }
        #endregion

        #region Handle context menu for showing uncertain

        /// <summary>
        /// Handle menu click on Show Uncertain.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniShowUncertain_Click(object sender, RoutedEventArgs e)
        {
            Configuration.ShowUncertain = this.mniShowUncertain.IsChecked;
            if (this._selectedElement != null)
            {
                this.HierarchyActions.RefreshHierarchy(false);
            }
        }

        /// <summary>
        /// set check state when menu is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniShowUncertain_Loaded(object sender, RoutedEventArgs e)
        {
            var mnu = ((MenuItem)sender);

            if (this.IsLiveMode == false)
            {
                mnu.Visibility = Visibility.Visible;
                mnu.IsChecked = Configuration.ShowUncertain;
            }
            else
            {
                mnu.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region load context menus for Tree view mode
        /// <summary>
        /// update Raw ctxmenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniRaw_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ElementContext == null || this.ElementContext.DataContext == null || this.ElementContext.DataContext.Mode != DataContextMode.Load)
            {
                this.mniRaw.IsEnabled = true;
                this.rbRaw.IsChecked = Configuration.TreeViewMode == TreeViewMode.Raw;
            }
            else
            {
                this.mniRaw.IsEnabled = false;
                this.rbRaw.IsChecked = this.ElementContext.Element.TreeWalkerMode == TreeViewMode.Raw;
            }
        }

        /// <summary>
        /// update Content ctx menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniContent_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ElementContext == null || this.ElementContext.DataContext == null || this.ElementContext.DataContext.Mode != DataContextMode.Load)
            {
                this.mniContent.IsEnabled = true;
                this.rbContent.IsChecked = Configuration.TreeViewMode == TreeViewMode.Content;
            }
            else
            {
                this.mniContent.IsEnabled = false;
                this.rbContent.IsChecked = this.ElementContext.Element.TreeWalkerMode == TreeViewMode.Content;
            }
        }

        /// <summary>
        /// update Control ctx menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ElementContext == null || this.ElementContext.DataContext == null || this.ElementContext.DataContext.Mode != DataContextMode.Load)
            {
                this.mniControl.IsEnabled = true;
                this.rbControl.IsChecked = Configuration.TreeViewMode == TreeViewMode.Control;
            }
            else
            {
                this.mniControl.IsEnabled = false;
                this.rbControl.IsChecked = this.ElementContext.Element.TreeWalkerMode == TreeViewMode.Control;
            }
        }

        /// <summary>
        /// Raw view is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniRaw_Click(object sender, RoutedEventArgs e)
        {
            RequestRefreshBasedOnTreeViewChange(TreeViewMode.Raw);
        }

        /// <summary>
        /// Content view is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniContent_Click(object sender, RoutedEventArgs e)
        {
            RequestRefreshBasedOnTreeViewChange(TreeViewMode.Content);
        }

        /// <summary>
        /// Control view is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniControl_Click(object sender, RoutedEventArgs e)
        {
            RequestRefreshBasedOnTreeViewChange(TreeViewMode.Control);
        }

        /// <summary>
        /// Handle Radion check event on Raw
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbRaw_Click(object sender, RoutedEventArgs e)
        {
            InvokeMenuItem(mniRaw);
        }

        /// <summary>
        /// Handle Radion check event on Control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbControl_Click(object sender, RoutedEventArgs e)
        {
            InvokeMenuItem(mniControl);
        }

        /// <summary>
        /// Handle Radion check event on Content
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbContent_Click(object sender, RoutedEventArgs e)
        {
            InvokeMenuItem(mniContent);
        }

        /// <summary>
        /// Invoke menu Item
        /// </summary>
        /// <param name="mn"></param>
        private void InvokeMenuItem(MenuItem mn)
        {
            var mnp = new MenuItemAutomationPeer(mn);
            var ip = mnp.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            ip.Invoke();
            SetFocusOnHierarchyTree();
        }

        /// <summary>
        /// Request Refresh at Tree view mode change.
        /// </summary>
        /// <param name="mode"></param>
        private void RequestRefreshBasedOnTreeViewChange(TreeViewMode mode)
        {
            Configuration.TreeViewMode = mode;
            SetTreeViewModeOnSelectAction(mode);
            SetFocusOnHierarchyTree();

            if (this._selectedElement != null)
            {
                // refresh tree automatically.
                this.HierarchyActions.RefreshHierarchy(true);
            }
        }

        private static void SetTreeViewModeOnSelectAction(TreeViewMode mode)
        {
            var selectAction = SelectAction.GetDefaultInstance();
            if (selectAction == null)
                return;

            selectAction.TreeViewMode = mode;
        }
        #endregion

        #region prevent auto-horizontal scroll
        private bool mSuppressRequestBringIntoView;
        private ElementContext ElementContext;

        private void TreeViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            // Ignore re-entrant calls
            if (mSuppressRequestBringIntoView)
                return;

            // Cancel the current scroll attempt
            e.Handled = true;

            // Call BringIntoView using a rectangle that extends into "negative space" to the left of our
            // actual control. This allows the vertical scrolling behaviour to operate without adversely
            // affecting the current horizontal scroll position.
            mSuppressRequestBringIntoView = true;

            TreeViewItem tvi = sender as TreeViewItem;
            if (tvi != null)
            {
                Rect newTargetRect = new Rect(-1000, 0, tvi.ActualWidth + 1000, tvi.ActualHeight);
                tvi.BringIntoView(newTargetRect);
            }

            mSuppressRequestBringIntoView = false;
        }

        /// <summary>
        // Correctly handle programmatically selected items
        // This updates the floating buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelected(object sender, RoutedEventArgs e)
        {
            var tvi = sender as TreeViewItem;
            var vm = tvi.DataContext as HierarchyNodeViewModel;
            if (vm != null)
            {
                btnMenu.DataContext = vm;

                if (vm.Element.IsRootElement())
                {
                    btnMenu.Visibility = Visibility.Collapsed;
                    btnTestElement.Visibility = Visibility.Collapsed;
                }
                else if (this.IsLiveMode)
                {
                    btnMenu.Visibility = Visibility.Visible;
                    btnTestElement.Visibility = Visibility.Visible;
                    var hlptxt = string.Format(CultureInfo.InvariantCulture,
                        Properties.Resources.HierarchyControl_LiveMode_InvokeToTestFormat,
                        vm.Element.Glimpse);
                    AutomationProperties.SetHelpText(btnTestElement, hlptxt);
                }
                else
                {
                    btnMenu.Visibility = Visibility.Visible;
                    btnTestElement.Visibility = Visibility.Collapsed;
                }
                tvi.BringIntoView();
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        GeneralTransform myTransform = tvi.TransformToAncestor(treeviewHierarchy);
                        System.Windows.Point p = myTransform.Transform(new System.Windows.Point(0, 0));

                        btnTestElement.Height = TreeButtonHeight;
                        btnTestElement.Margin = new Thickness(0, p.Y, btnTestElement.Margin.Right, 0);
                        btnMenu.Height = TreeButtonHeight;
                        btnMenu.Margin = new Thickness(0, p.Y, btnMenu.Margin.Right, 0);
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception ex)
                    {
                        ex.ReportException();
                    }
#pragma warning restore CA1031 // Do not catch general exception types
                }));
            }

            e.Handled = true;
        }
        #endregion

        /// <summary>
        /// Handle Context Menu : Expand all
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var node = (HierarchyNodeViewModel)((System.Windows.FrameworkElement)sender).DataContext;

            if (node.NeedSnapshot && node.Element is DesktopElement && node.Element.IsRootElement() == false)
            {
                Dispatcher.Invoke(() =>
                {
                    var sa = SelectAction.GetDefaultInstance();
                    sa.SetCandidateElement(this.ElementContext.Id, node.Element.UniqueId);
                    sa.Select();
                    this.HierarchyActions.RefreshHierarchy(true);
                });
            }
            else
            {
                Logger.PublishTelemetryEvent(TelemetryAction.Hilighter_Expand_AllDescendants);
                node.Expand(true);
            }
        }

        /// <summary>
        /// Set handler to test the selected element node on hierarchy
        /// </summary>
        /// <param name="handler"></param>
        public void SetbtnTestElementHandler(RoutedEventHandler handler)
        {
            btnTestElement.Click += handler;
        }

        /// <summary>
        /// Keep event button inside of scrollview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeviewHierarchy_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // set scroll bar location info.
            var sv = e.OriginalSource as ScrollViewer;
            int right;

            if (sv.ComputedVerticalScrollBarVisibility == Visibility.Visible)
            {
                right = 20;
                treeviewHierarchy.Margin = new Thickness(0);
            }
            else
            {
                right = 4;
                treeviewHierarchy.Margin = new Thickness(0, 0, 2, 0);
            }

            /// set button locations too.
            double height = TreeButtonHeight;

            if (sv.ComputedHorizontalScrollBarVisibility == Visibility.Visible)
            {
                var diff = btnMenu.Margin.Top - e.VerticalChange - treeviewHierarchy.ActualHeight + TreeButtonVerticalSpacing;
                height = diff > 0 ? diff > TreeButtonHeight ? 0 : TreeButtonHeight - diff : TreeButtonHeight;
            }

            btnMenu.Height = height;
            btnTestElement.Height = height;

            btnMenu.Margin = new Thickness(0, btnMenu.Margin.Top - e.VerticalChange, right, 0);
            btnTestElement.Margin = new Thickness(0, btnTestElement.Margin.Top - e.VerticalChange, right + TreeButtonHorizontalSpacing, 0);
        }

        /// <summary>
        /// Focus on treeview
        /// </summary>
        public new void Focus()
        {
            this.treeviewHierarchy.Focus();
        }

        public void FileBug(HierarchyNodeViewModel vm = null)
        {
            vm = vm ?? this.treeviewHierarchy.SelectedItem as HierarchyNodeViewModel;

            if (vm == null)
            {
                MessageDialog.Show(Properties.Resources.HierarchyControl_FileBug_Could_not_find_the_selected_item__the_bug_filing_is_canceled);
                return;
            }

            var input = new FileIssueWrapperInput(
                vm,
                this.ElementContext.Id,
                this.HierarchyActions.SwitchToServerLogin,
                () => this._selectedElement.GetIssueInformation(IssueType.NoFailure),
                FileBugRequestSource.Hierarchy);
            FileIssueWrapper.FileIssueFromControl(input);
        }

        /// <summary>
        /// Handles click on file bug button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniFileBug_Click(object sender, RoutedEventArgs e)
        {
            var vm = ((MenuItem)sender).DataContext as HierarchyNodeViewModel;
            FileBug(vm);
        }

        /// <summary>
        /// Enter events mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniEvents_Click(object sender, RoutedEventArgs e)
        {
            var vm = (sender as MenuItem).DataContext as HierarchyNodeViewModel;
            var sa = SelectAction.GetDefaultInstance();
            sa.SetCandidateElement(this.ElementContext.Id, vm.Element.UniqueId);
            sa.Select();
            this.HierarchyActions.HandleLiveToEvents(sa.POIElementContext.Element);
        }

        /// <summary>
        /// File bug front live mode click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniFileBugLive_Click(object sender, RoutedEventArgs e)
        {
            var vm = (sender as MenuItem).DataContext as HierarchyNodeViewModel;
            var sa = SelectAction.GetDefaultInstance();
            sa.SetCandidateElement(this.ElementContext.Id, vm.Element.UniqueId);
            sa.Select();
            this.HierarchyActions.HandleFileBugLiveMode();
        }

        /// <summary>
        /// Fix down arrow key behavior on buttons floating over hierarchy tree
        /// </summary>
        private void FloatingButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up)
            {
                treeviewHierarchy.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                e.Handled = true;
            }
        }
    }
}
