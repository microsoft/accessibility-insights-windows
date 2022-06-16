// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.SharedUx.Controls.CustomControls;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.ViewModels;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Results;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Documents;

namespace AccessibilityInsights.SharedUx.Controls
{
    /// <summary>
    /// Interaction logic for ScannerResultControl.xaml
    /// </summary>
    public partial class ScannerResultControl : UserControl
    {
        private readonly List<ScanListViewItemViewModel> _list;

        /// <summary>
        /// Action to perform when user needs to log into the server
        /// </summary>
        public Action SwitchToServerLogin { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ScannerResultControl()
        {
            InitializeComponent();
            _list = new List<ScanListViewItemViewModel>();
            Resources.Source = new Uri(@"pack://application:,,,/AccessibilityInsights.SharedUx;component/Resources/Styles.xaml", UriKind.Absolute);
        }

        /// <summary>
        /// Current ecid
        /// </summary>
        private Guid EcId;

        public IList<ScanListViewItemViewModel> List => _list;
        public A11yElement Element { get; private set; }

        /// <summary>
        /// Style dictionary
        /// </summary>
        new ResourceDictionary Resources = new ResourceDictionary();

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
        /// Show all scan results
        /// </summary>
        bool ShowAllResults;

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "pane");
        }

        /// <summary>
        /// Set Test data
        /// </summary>
        /// <param name="e"></param>
        public void SetElement(A11yElement e, Guid ecId)
        {
            this.Clear();
            if (e != null && e.ScanResults != null && e.ScanResults.Items.Count != 0)
            {
                this.EcId = ecId;
                this.Element = e;
                SetScannerResultTreeView(e);
            }
        }

        /// <summary>
        /// Set data on the UI
        /// </summary>
        private void SetScannerResultTreeView(A11yElement e)
        {
            this.nonFrameworkListControl.SetControlContext(new ScannerResultCustomListContext(UpdateTree, SwitchToServerLogin, ChangeVisibility, spHowToFix, this.EcId));
            this.frameworkListControl.SetControlContext(new ScannerResultCustomListContext(UpdateTree, SwitchToServerLogin, ChangeVisibility, spHowToFix, this.EcId));
            _list.AddRange(ScanListViewItemViewModel.GetScanListViewItemViewModels(e));
            this.nonFrameworkListControl.ItemsSource = null;
            this.frameworkListControl.ItemsSource = null;

            // enable UI elements since Clear() disables them.
            this.btnShowAll.IsEnabled = true;

            UpdateTree();
        }

        /// <summary>
        /// Update tree, list and button text
        /// </summary>
        private void UpdateTree()
        {
            var itemViewModel = from l in this.List
                                where l.Status == ScanStatus.Fail || l.Status == ScanStatus.ScanNotSupported || (Configuration.ShowUncertain && l.Status == ScanStatus.Uncertain) || ShowAllResults
                                orderby l.Status descending, l.Source, l.Header
                                select l;

            List<ScanListViewItemViewModel> frameworkIssues = new List<ScanListViewItemViewModel>();
            List<ScanListViewItemViewModel> nonFrameworkIssues = new List<ScanListViewItemViewModel>();
            SplitResultList(itemViewModel, frameworkIssues, nonFrameworkIssues);

            var viewModelCount = itemViewModel.Count();

            this.nonFrameworkListControl.ItemsSource = nonFrameworkIssues;
            this.frameworkListControl.ItemsSource = frameworkIssues;

            btnShowAll.Visibility = Visibility.Visible;

            if (!ShowAllResults)
            {
                int diff = this.List.Count - viewModelCount;
                if (diff > 0)
                {
                    tbShowAll.Text = String.Format(CultureInfo.InvariantCulture, "{0} ({1})", Configuration.ShowUncertain ? Properties.Resources.ScannerResultControl_UpdateTree_Passed_tests : Properties.Resources.ScannerResultControl_UpdateTree_Passed_and_Uncertain_tests, diff);
                    this.btnShowAll.Visibility = Visibility.Visible;
                }
                else
                {
                    this.btnShowAll.Visibility = Visibility.Collapsed;
                }
            }

            if (viewModelCount > 0)
            {
                nonFrameworkListControl.lvDetails.SelectedItem = 0;
                frameworkListControl.lvDetails.SelectedItem = 0;
                this.spHowToFix.DataContext = itemViewModel.First<ScanListViewItemViewModel>();
            }
            else
            {
                this.spHowToFix.DataContext = null;
            }
            this.ShowAllResults = false;
        }

        private static void SplitResultList(IEnumerable<ScanListViewItemViewModel> results, List<ScanListViewItemViewModel> frameworkIssues, List<ScanListViewItemViewModel> nonFrameworkIssues)
        {
            foreach (var result in results)
            {
                if (result.RR.FrameworkIssueLink == null)
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
        /// Clear control
        /// </summary>
        public void Clear()
        {
            this.nonFrameworkListControl.ItemsSource = null;
            this.frameworkListControl.ItemsSource = null;
            this.List.Clear();
            this.tbShowAll.Text = Properties.Resources.NoTestResult;
            this.btnShowAll.IsEnabled = false;
        }

        /// <summary>
        /// Handles snippet click
        /// </summary>
        private void HyperlinkSnippetClick(object sender, EventArgs e)
        {
            ((sender as Hyperlink).DataContext as ScanListViewItemViewModel).InvokeSnippetLink();
        }

        /// <summary>
        /// Show passed scan results
        /// </summary>
        private void BtnShowAll_Click(object sender, RoutedEventArgs e)
        {
            this.ShowAllResults = true;
            UpdateTree();
            (sender as Button).Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Change visibility of scanner results details
        /// </summary>
        public void ChangeVisibility()
        {
            var visible = this.btnShowAll.Visibility;
            this.ShowAllResults = visible == Visibility.Collapsed;
            UpdateTree();
            this.btnShowAll.Visibility = visible;
        }
    }
}