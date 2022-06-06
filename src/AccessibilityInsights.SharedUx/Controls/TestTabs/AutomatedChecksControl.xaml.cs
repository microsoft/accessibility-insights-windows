// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.Controls.CustomControls;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.FileIssue;
using AccessibilityInsights.SharedUx.Highlighting;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.SharedUx.ViewModels;
using Axe.Windows.Actions.Contexts;
using Axe.Windows.Core.Bases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace AccessibilityInsights.SharedUx.Controls.TestTabs
{
    /// <summary>
    /// Interaction logic for AutomatedChecksControl.xaml
    /// </summary>
    public partial class AutomatedChecksControl : UserControl
    {
        /// <summary>
        /// Element context
        /// </summary>
        public ElementContext ElementContext { get; set; }

        /// <summary>
        /// Delegate for rerun test
        /// </summary>
        public Action RunAgainTest { get; set; }

        /// <summary>
        /// Data context
        /// </summary>
        new ElementDataContext DataContext;

        /// <summary>
        /// Action to perform when element clicked on in listview
        /// </summary>
        public Action NotifyElementSelected { get; set; }

        /// <summary>
        /// Action to perform when user needs to log into the server
        /// </summary>
        public Action SwitchToServerLogin { get; set; }

        /// <summary>
        /// Overriding LocalizedControlType
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "pane");
        }

        /// <summary>
        /// App configation
        /// </summary>
        public static ConfigurationModel Configuration
        {
            get
            {
                return ConfigurationManager.GetDefaultInstance()?.AppConfig;
            }
        }

        /// <summary>
        /// Whether there is a screenshot available (if not, checkboxes should be disabled)
        /// </summary>
        public bool ScreenshotAvailable => DataContext != null && DataContext.Screenshot != null;

        /// <summary>
        /// Gets/sets fastpass highlighter visibility, updating as necessary
        /// </summary>
#pragma warning disable CA1822
        public bool HighlightVisibility
#pragma warning restore CA1822
        {
            get
            {
                return Configuration.IsHighlighterOn;
            }
            set
            {
                Configuration.IsHighlighterOn = value;

                var ha = ImageOverlayDriver.GetDefaultInstance();

                if (value)
                {
                    ha.Show();
                    foreach (var svm in SelectedItems)
                    {
                        ha.AddElement(this.ElementContext.Id, svm.Element.UniqueId);
                    }
                    Application.Current.MainWindow.Activate();
                }
                else
                {
                    ha.Hide();
                }
            }
        }

        /// <summary>
        /// Currently selected items
        /// </summary>
        IList<RuleResultViewModel> SelectedItems = new List<RuleResultViewModel>();

        /// <summary>
        /// Set results and populate UI
        /// </summary>
        /// <param name="results"></param>
        public void SetRuleResults(IList<RuleResultViewModel> results)
        {
            if (results != null)
            {
                this.lvResults2.ListView.IsEnabled = true;
                this.lvResults2.ListView.ItemsSource = results;
                this.lvResults2.ListView.Visibility = Visibility.Visible;
                this.lblCongrats.Visibility = Visibility.Collapsed;
                this.lblNoFail.Visibility = Visibility.Collapsed;
                this.gdFailures.Visibility = Visibility.Visible;
                this.gdFailures.Focus();
                var count = results.Where(r => r.RuleResult.Status == Axe.Windows.Core.Results.ScanStatus.Fail).Count();

                switch (count)
                {
                    case 0:
                        this.runFailures.Text = Properties.Resources.AutomatedChecksControl_SetRuleResults_No_failure_was;
                        break;
                    case 1:
                        this.runFailures.Text = Properties.Resources.AutomatedChecksControl_SetRuleResults_1_failure_was;
                        break;
                    default:
                        this.runFailures.Text = String.Format(CultureInfo.InvariantCulture, Properties.Resources.AutomatedChecksControl_SetRuleResults_0_failures_were, results.Count);
                        break;
                }

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvResults2.ListView.ItemsSource);
                PropertyGroupDescription groupDescription = new PropertyGroupDescription(Properties.Resources.AutomatedChecksControl_SetRuleResults_TitleURL);
                view.GroupDescriptions.Add(groupDescription);
            }
            else
            {
                this.gdFailures.Visibility = Visibility.Collapsed;
                this.lvResults2.ListView.ItemsSource = null;
                this.lblCongrats.Visibility = Visibility.Visible;
                this.lblNoFail.Visibility = Visibility.Visible;
                this.lvResults2.ListView.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Automated Checks mode is left
        /// </summary>
        public static void Hide()
        {
            ImageOverlayDriver.GetDefaultInstance().ClearElements();
        }

        /// Automated Checks mode is shown
        public void Show()
        {
            var ha = ImageOverlayDriver.GetDefaultInstance();
            // set handler here. it will make sure that highliter button is shown and working.
            ha.SetHighlighterButtonClickHandler(TBElem_Click);

            if (this.HighlightVisibility)
            {
                ha.Show();

                foreach (var svm in SelectedItems)
                {
                    ha.AddElement(this.ElementContext.Id, svm.Element.UniqueId);
                }

                /// Only activate the main window if tests have already been run. This ensures we don't lose transient UI.
                /// We need to activate the window so that keyboard focus isn't lost when switching from Tab Stops to
                /// Automated Checks.
                if (this.ElementContext != null)
                {
                    Application.Current.MainWindow.Activate();
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public AutomatedChecksControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles click on error exclamation point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TBElem_Click(object sender, RoutedEventArgs e)
        {
            NotifySelected(((TextBlock)sender).Tag as A11yElement);
        }

        /// <summary>
        /// Notify Element selection to swith mode to snapshot
        /// </summary>
        /// <param name="e"></param>
        private void NotifySelected(A11yElement e)
        {
            this.DataContext.FocusedElementUniqueId = e.UniqueId;
            NotifyElementSelected();
        }

        /// <summary>
        /// Sets element context and updates UI
        /// </summary>
        /// <param name="ec"></param>
        public void SetElement(ElementContext ec)
        {
            if (ec == null)
                throw new ArgumentNullException(nameof(ec));

            try
            {
                // set handler here. it will make sure that highliter button is shown and working.
                ImageOverlayDriver.GetDefaultInstance().SetHighlighterButtonClickHandler(TBElem_Click);

                if (this.ElementContext == null || ec.Element != this.ElementContext.Element || this.DataContext != ec.DataContext)
                {
                    var viewModel = new AutomatedChecksCustomListViewModel(lvResults2, ec.DataContext, NotifyElementSelected,
                        SwitchToServerLogin)
                    {
                        ElementContext = ec,
                    };
                    
                    this.lblCongrats.Visibility = Visibility.Collapsed;
                    this.lblNoFail.Visibility = Visibility.Collapsed;
                    this.gdFailures.Visibility = Visibility.Collapsed;
                    this.DataContext = ec.DataContext;
                    this.lvResults2.ViewModel = viewModel;
                    this.lvResults2.CheckBoxSelectAll.IsEnabled = ScreenshotAvailable;
                    this.lvResults2.ListView.ItemsSource = null;
                    this.ElementContext = ec;
                    this.tbGlimpse.Text = string.Format(CultureInfo.InvariantCulture,
                        Properties.Resources.AutomatedChecksControl_TargetFormat, ec.Element.Glimpse);
                    UpdateUI();
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                e.ReportException();
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Displays loading indicator and updates UI
        /// </summary>
        /// <param name="forceRefresh"></param>
        public void UpdateUI()
        {
            this.lblCongrats.Visibility = Visibility.Collapsed;
            this.lblNoFail.Visibility = Visibility.Collapsed;
            this.gdFailures.Visibility = Visibility.Collapsed;
            this.lvResults2.Reset();
            this.SelectedItems.Clear();
            this.lvResults2.CheckBoxSelectAll.IsChecked = false;
            HollowHighlightDriver.GetDefaultInstance().Clear();

            if (this.DataContext != null)
            {
                var list = this.DataContext.GetRuleResultsViewModelList();

                SetRuleResults(list);

                if (list != null)
                {
                    var ha = ImageOverlayDriver.GetDefaultInstance();
                    ha.SetImageElement(ElementContext.Id);
                    if (HighlightVisibility)
                    {
                        ha.Show();
                    }
                }

                Dispatcher.Invoke(() =>
                {
                    lvResults2.ViewModel.CheckAllBoxes(lvResults2.ListView, true);
                }, DispatcherPriority.Input);
            }
        }

        /// <summary>
        /// Clears listview
        /// </summary>
        public void ClearUI()
        {
            ImageOverlayDriver.ClearDefaultInstance();
            this.ElementContext = null;
            this.lvResults2.ListView.ItemsSource = null;
            this.tbGlimpse.Text = Properties.Resources.GlimpseTextTarget;
            this.gdFailures.Visibility = Visibility.Collapsed;
            this.SelectedItems.Clear();
        }

        /// <summary>
        /// Rescanns current element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRescan_Click(object sender, RoutedEventArgs e)
        {
            this.lvResults2.ListView.IsEnabled = false;
            this.RunAgainTest();
        }

        /// <summary>
        /// Add horizontal scroll bar when width is too narrow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scrollview_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var view = sender as ScrollViewer;
            if (e.NewSize.Width <= this.tbSubTitle.MinWidth)
            {
                view.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                this.tbSubTitle.Width = this.tbSubTitle.MinWidth;
            }
            else
            {
                view.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                this.tbSubTitle.Width = Double.NaN;
            }
        }

        ///// <summary>
        ///// Handles unselecting a listviewitem
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void ListViewItem_Unselected(object sender, RoutedEventArgs e)
        //{
        //    var lvi = sender as ListViewItem;
        //    var exp = GetParentElem<Expander>(lvi) as Expander;
        //    var cb = GetFirstChildElement<CheckBox>(exp) as CheckBox;
        //    var srvm = lvi.DataContext as RuleResultViewModel;

        //    // ElementContext can be null when app is closed.
        //    if (this.ElementContext != null)
        //    {
        //        ImageOverlayDriver.GetDefaultInstance().RemoveElement(this.ElementContext.Id, srvm.Element.UniqueId);
        //    }

        //    SelectedItems.Remove(srvm);
        //    var groupitem = GetParentElem<GroupItem>(exp) as GroupItem;
        //    var dc = cb.DataContext as CollectionViewGroup;
        //    var itms = dc.Items;
        //    var any = itms.Intersect(SelectedItems).Any();
        //    if (any)
        //    {
        //        cb.IsChecked = null;
        //        groupitem.Tag = "some";
        //    }
        //    else
        //    {
        //        cb.IsChecked = false;
        //        groupitem.Tag = "zero";
        //    }
        //    UpdateSelectAll();
        //}

        ///// <summary>
        ///// Handles list view item selection
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        //{
        //    if (ScreenshotAvailable)
        //    {
        //        var lvi = sender as ListViewItem;
        //        var exp = GetParentElem<Expander>(lvi) as Expander;
        //        var cb = GetFirstChildElement<CheckBox>(exp) as CheckBox;
        //        var itm = lvi.DataContext as RuleResultViewModel;
        //        if (!SelectedItems.Contains(itm))
        //        {
        //            SelectedItems.Add(itm);
        //            UpdateSelectAll();
        //            ImageOverlayDriver.GetDefaultInstance().AddElement(this.ElementContext.Id, itm.Element.UniqueId);
        //        }
        //        var groupitem = GetParentElem<GroupItem>(exp) as GroupItem;
        //        var dc = cb.DataContext as CollectionViewGroup;
        //        var itms = dc.Items;
        //        var any = itms.Except(SelectedItems).Any();
        //        if (any)
        //        {
        //            cb.IsChecked = null;
        //            groupitem.Tag = "some"; // used to indicate how many children are selected
        //        }
        //        else
        //        {
        //            cb.IsChecked = true;
        //            groupitem.Tag = "all";
        //        }
        //    }
        //}

        /// <summary>
        /// Call show/hide when tab changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
#pragma warning disable CA1801 // unused parameter
        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
#pragma warning restore CA1801 // unused parameter
        {
            if ((bool)e.NewValue == true)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        /// <summary>
        /// Handles click on show failures button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowFailures_Click(object sender, RoutedEventArgs e)
        {
            this.HighlightVisibility = !this.HighlightVisibility;
        }

        /// <summary>
        /// Set highlighter toggle off
        /// </summary>
        private void ToggleHighlighterOff()
        {
            this.HighlightVisibility = false;
        }

        /// <summary>
        /// Click on view results in UIA tree button goes to Test Inspect view with POI element selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTree_Click(object sender, RoutedEventArgs e)
        {
            NotifySelected(ElementContext.Element);
        }
    }
}
