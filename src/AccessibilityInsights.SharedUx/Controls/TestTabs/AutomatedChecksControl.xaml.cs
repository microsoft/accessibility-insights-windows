// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.SharedUx.Controls.CustomControls;
using AccessibilityInsights.SharedUx.Highlighting;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.SharedUx.ViewModels;
using Axe.Windows.Actions.Contexts;
using Axe.Windows.Core.Bases;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
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
        /// App configuration
        /// </summary>
        public static ConfigurationModel Configuration
        {
            get
            {
                return ConfigurationManager.GetDefaultInstance()?.AppConfig;
            }
        }

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
                    foreach (var svm in nonFrameworkListControl.SelectedItems)
                    {
                        ha.AddElement(this.ElementContext.Id, svm.Element.UniqueId);
                    }
                    foreach (var svm in frameworkListControl.SelectedItems)
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
        /// Set results and populate UI
        /// </summary>
        /// <param name="results"></param>
        public void SetRuleResults(IList<RuleResultViewModel> results)
        {
            if (results != null)
            {
                List<RuleResultViewModel> frameworkIssues = new List<RuleResultViewModel>();
                List<RuleResultViewModel> nonFrameworkIssues = new List<RuleResultViewModel>();
                SplitResultList(results, frameworkIssues, nonFrameworkIssues);

                this.nonFrameworkListControl.SetItemsSource(nonFrameworkIssues.Any() ? nonFrameworkIssues : null);
                this.frameworkListControl.SetItemsSource(frameworkIssues.Any() ? frameworkIssues : null);

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

                PropertyGroupDescription groupDescription = new PropertyGroupDescription(Properties.Resources.AutomatedChecksControl_SetRuleResults_TitleURL);
                if (frameworkIssues.Any())
                {
                    frameworkListControl.AddGroupDescription(groupDescription);
                }
                if (nonFrameworkIssues.Any())
                {
                    nonFrameworkListControl.AddGroupDescription(groupDescription);
                }
            }
            else
            {
                this.gdFailures.Visibility = Visibility.Collapsed;
                this.lblCongrats.Visibility = Visibility.Visible;
                this.lblNoFail.Visibility = Visibility.Visible;
                this.nonFrameworkListControl.SetItemsSource(null);
                this.frameworkListControl.SetItemsSource(null);
            }
        }

        private static void SplitResultList(IEnumerable<RuleResultViewModel> results, IList<RuleResultViewModel> frameworkIssues, IList<RuleResultViewModel> nonFrameworkIssues)
        {
            foreach (var result in results)
            {
                if (result.RuleResult.FrameworkIssueLink == null)
                {
                    nonFrameworkIssues.Add(result);
                }
                else
                {
                    frameworkIssues.Add(result);
                }
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
            // set handler here. It will make sure that highlighter button is shown and working.
            ha.SetHighlighterButtonClickHandler(TBElem_Click);

            if (this.HighlightVisibility)
            {
                ha.Show();

                foreach (var svm in nonFrameworkListControl.SelectedItems)
                {
                    ha.AddElement(this.ElementContext.Id, svm.Element.UniqueId);
                }
                foreach (var svm in frameworkListControl.SelectedItems)
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
        /// Notify Element selection to switch mode to snapshot
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
        public void SetElement(ElementContext ec)
        {
            if (ec == null)
                throw new ArgumentNullException(nameof(ec));

            try
            {
                // set handler here. it will make sure that highlighter button is shown and working.
                ImageOverlayDriver.GetDefaultInstance().SetHighlighterButtonClickHandler(TBElem_Click);

                if (this.ElementContext == null || ec.Element != this.ElementContext.Element || this.DataContext != ec.DataContext)
                {
                    var controlContext = new AutomatedChecksCustomListControlContext(ec, NotifyElementSelected, SwitchToServerLogin);
                    this.nonFrameworkListControl.SetControlContext(controlContext);
                    this.frameworkListControl.SetControlContext(controlContext);
                    this.nonFrameworkListControl.SetItemsSource(null);
                    this.frameworkListControl.SetItemsSource(null);

                    this.lblCongrats.Visibility = Visibility.Collapsed;
                    this.lblNoFail.Visibility = Visibility.Collapsed;
                    this.gdFailures.Visibility = Visibility.Collapsed;
                    this.DataContext = ec.DataContext;
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
        public void UpdateUI()
        {
            this.lblCongrats.Visibility = Visibility.Collapsed;
            this.lblNoFail.Visibility = Visibility.Collapsed;
            this.gdFailures.Visibility = Visibility.Collapsed;
            this.nonFrameworkListControl.Reset();
            this.frameworkListControl.Reset();
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
                    nonFrameworkListControl.CheckAllBoxes();
                    frameworkListControl.CheckAllBoxes();
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
            this.nonFrameworkListControl.Reset();
            this.frameworkListControl.Reset();
            this.tbGlimpse.Text = Properties.Resources.GlimpseTextTarget;
            this.gdFailures.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Add horizontal scroll bar when width is too narrow
        /// </summary>
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

        /// <summary>
        /// Call show/hide when tab changes
        /// </summary>
        private void UserControl_IsVisibleChanged(object _, DependencyPropertyChangedEventArgs e)
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
        private void btnShowFailures_Click(object sender, RoutedEventArgs e)
        {
            this.HighlightVisibility = !this.HighlightVisibility;
        }

        /// <summary>
        /// Click on view results in UIA tree button goes to Test Inspect view with POI element selected
        /// </summary>
        private void btnTree_Click(object sender, RoutedEventArgs e)
        {
            NotifySelected(ElementContext.Element);
        }
    }
}
