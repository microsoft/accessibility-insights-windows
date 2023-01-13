// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Enums;
using AccessibilityInsights.Extensions.Interfaces.Upgrades;
using AccessibilityInsights.SetupLibrary;
using AccessibilityInsights.SharedUx.Highlighting;
using AccessibilityInsights.SharedUx.Interfaces;
using AccessibilityInsights.SharedUx.Misc;
using AccessibilityInsights.SharedUx.Settings;
using Axe.Windows.Actions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;

namespace AccessibilityInsights
{
    /// <summary>
    /// MainWindow partial class for Helping flow control
    /// </summary>
    public partial class MainWindow : ICCAMode
    {
        /// <summary>
        /// flag to allow any further action
        /// </summary>
        public bool AllowFurtherAction { get; set; } = true;

        /// <summary>
        /// Enable appropriate Tracker in SelectAction based on configuration
        /// Keyboard focus vs. Mouse
        /// allow enable only if selection is feasible based on mode.
        /// </summary>
        internal void EnableElementSelector()
        {
            var sa = SelectAction.GetDefaultInstance();
            if (!sa.IsPaused &&
                ((CurrentPage == AppPage.Inspect && (InspectView)CurrentView == InspectView.Live)
                ||
                (CurrentPage == AppPage.CCA && (CCAView)CurrentView == CCAView.Automatic)
                ))
            {
                this.AllowFurtherAction = true;
                sa.Start();
                this.timerSelector?.Start();
            }
        }

        /// <summary>
        /// Disable all element selector
        /// </summary>
        internal void DisableElementSelector()
        {
            var sa = SelectAction.GetDefaultInstance();
            if (!sa.IsPaused)
            {
                this.AllowFurtherAction = false;
                this.timerSelector?.Stop(); // make sure selection timer is disabled.
                sa.Stop(); // stop selector action.
            }
        }

        /// <summary>
        /// Update MainWindowUI based on current mode/view
        /// - title
        /// - highlighter
        /// - command bar
        /// </summary>
        internal void UpdateMainWindowUI()
        {
            SetFontSize();
            UpdateMainCommandButtons();
            UpdateTabSelection();
            UpdateTitleString();
            UpdateMainWindowConnectionFields();
        }

        /// <summary>
        /// Hide all crumbs
        /// </summary>
        private void CollapseAllCrumbs()
        {
            btnCrumbOne.Visibility = Visibility.Collapsed;
            btnCrumbTwo.Visibility = Visibility.Collapsed;
            tbCrumbOne.Visibility = Visibility.Collapsed;
            tbCrumbTwo.Visibility = Visibility.Collapsed;
            tbCrumbThree.Visibility = Visibility.Collapsed;
            ctrlFabCrumbOne.Visibility = Visibility.Collapsed;
            ctrlFabCrumbTwo.Visibility = Visibility.Collapsed;
        }

        private static void SetVisibleText(FrameworkElement control, string content = "")
        {
            control.Visibility = Visibility.Visible;

            if (control is Button)
            {
                ((Button)control).Content = content;
            }
            else if (control is TextBlock)
            {
                ((TextBlock)control).Text = content;
            }
        }

        /// <summary>
        /// Set the text and visibility of breadcrumb buttons based on current view
        /// </summary>
        private void UpdateBreadcrumbs()
        {
            var sa = SelectAction.GetDefaultInstance();
            _ = new ResourceDictionary
            {
                Source = new Uri(@"pack://application:,,,/AccessibilityInsights.SharedUx;component/Resources/Styles.xaml", UriKind.Absolute)
            };

            CollapseAllCrumbs();

            switch (this.CurrentPage)
            {
                case AppPage.Events:
                    SetVisibleText(btnCrumbOne, Properties.Resources.liveInspect);
                    SetVisibleText(ctrlFabCrumbOne);
                    if (sa.IsPaused)
                    {
                        SetVisibleText(btnCrumbTwo, Properties.Resources.paused);
                        SetVisibleText(ctrlFabCrumbTwo);
                        SetVisibleText(tbCrumbThree, Properties.Resources.events);
                    }
                    else
                    {
                        SetVisibleText(tbCrumbTwo, Properties.Resources.events);
                    }
                    break;
                case AppPage.Inspect:
                    if (sa.IsPaused)
                    {
                        SetVisibleText(btnCrumbOne, Properties.Resources.liveInspect);
                        SetVisibleText(ctrlFabCrumbOne);
                        SetVisibleText(tbCrumbTwo, Properties.Resources.paused);
                    }
                    else
                    {
                        SetVisibleText(tbCrumbOne, Properties.Resources.liveInspect);
                    }
                    break;
                case AppPage.Test:
                    switch ((TestView)this.CurrentView)
                    {
                        case TestView.NoSelection:
                        case TestView.CapturingData:
                        case TestView.AutomatedTestResults:
                        case TestView.TabStop:
                            SetVisibleText(btnCrumbOne, Properties.Resources.liveInspect);
                            SetVisibleText(ctrlFabCrumbOne);
                            if (sa.IsPaused)
                            {
                                SetVisibleText(btnCrumbTwo, Properties.Resources.paused);
                                SetVisibleText(ctrlFabCrumbTwo);
                                SetVisibleText(tbCrumbThree, Properties.Resources.tests);
                            }
                            else
                            {
                                SetVisibleText(tbCrumbTwo, Properties.Resources.tests);
                            }
                            break;
                        case TestView.ElementDetails:
                        case TestView.ElementHowToFix:
                            SetVisibleText(btnCrumbOne, Properties.Resources.liveInspect);
                            SetVisibleText(ctrlFabCrumbOne);
                            SetVisibleText(btnCrumbTwo, Properties.Resources.tests);
                            SetVisibleText(ctrlFabCrumbTwo);
                            SetVisibleText(tbCrumbThree, Properties.Resources.resultsInUIATree);
                            break;
                        default:
                            break;
                    }
                    break;
                case AppPage.CCA:
                    SetVisibleText(btnCrumbOne, Properties.Resources.liveInspect);
                    SetVisibleText(ctrlFabCrumbOne);
                    SetVisibleText(tbCrumbTwo, Properties.Resources.colorContrast);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Update main command buttons
        /// </summary>
        private void UpdateMainCommandButtons()
        {
            if (this.CurrentPage != AppPage.Start)
            {
                this.ctrlNamedCommandbar.Visibility = Visibility.Visible;
            }

            this.btnHilighter.Visibility = this.CurrentPage == AppPage.CCA ? Visibility.Collapsed : Visibility.Visible;
            this.btnRefresh.Visibility = this.ctrlCurMode.IsRefreshEnabled ? Visibility.Visible : Visibility.Collapsed;
            this.btnSave.Visibility = this.ctrlCurMode.IsSaveEnabled ? Visibility.Visible : Visibility.Collapsed;
            this.btnLoad.Visibility = this.IsInSelectingState() ? Visibility.Visible : Visibility.Collapsed;
            this.tbComboboxLabel.Visibility = this.IsInSelectingState() ? Visibility.Visible : Visibility.Collapsed;
            this.btnTimer.Visibility = this.IsInSelectingState() ? Visibility.Visible : Visibility.Collapsed;
            this.cbSelectionScope.Visibility = this.IsInSelectingState() ? Visibility.Visible : Visibility.Collapsed;
            this.btnPause.Visibility = (this.CurrentPage == AppPage.Inspect) && (this.gridlayerConfig.Visibility == Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed;

            // add n of m info to UIA name based on currently visible focusable controls
            var visibleCommands = new List<UIElement>();
            foreach (UIElement child in this.spCommandBar.Children)
            {
                if (child.Visibility == Visibility.Visible && child.Focusable)
                {
                    visibleCommands.Add(child);
                }
            }

            int count = 1;
            foreach (var elem in visibleCommands)
            {
                elem.SetValue(AutomationProperties.PositionInSetProperty, count);
                elem.SetValue(AutomationProperties.SizeOfSetProperty, visibleCommands.Count);
                count++;
            }
        }

        /// <summary>
        /// Update Highlighter of the selected Tab on left side.
        /// </summary>
        private void UpdateTabSelection()
        {
            cvsInspect.Visibility = Visibility.Collapsed;
            cvsTest.Visibility = Visibility.Collapsed;
            cvsCCA.Visibility = Visibility.Collapsed;

            switch (this.CurrentPage)
            {
                case AppPage.Events:
                case AppPage.Inspect:
                    cvsInspect.Visibility = Visibility.Visible;
                    break;
                case AppPage.Test:
                    cvsTest.Visibility = Visibility.Visible;
                    break;
                case AppPage.CCA:
                    cvsCCA.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
            this.UpdateNavigationBarAutomationName();
        }

        /// <summary>
        /// Set the title string for Main window.
        /// Update Automation Property Name too.
        /// </summary>
        /// <returns></returns>
        private void UpdateTitleString()
        {
            this.Title = string.Format(CultureInfo.InvariantCulture, Properties.Resources.MainWindow_UpdateTitleString_Accessibility_Insights_for_Windows_0, GetCurrentStateTextBasedOnCurrentPageAndCurrentView());
            UpdateBreadcrumbs();
        }

        /// <summary>
        /// Set version text, per https://github.com/microsoft/accessibility-insights-windows/issues/347
        /// </summary>
        private void UpdateVersionString()
        {
            string content = ComputeVersionBarString();

            this.lblVersion.Content = content;
            this.lblVersion.Visibility = content.Length > 0 ?
                Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Compute version text, per https://github.com/microsoft/accessibility-insights-windows/issues/347
        /// </summary>
        private string ComputeVersionBarString()
        {
            ReleaseChannel? channel = ConfigurationManager.GetDefaultInstance()?.AppConfig?.ReleaseChannel;
            if ((channel.HasValue && channel.Value != ReleaseChannel.Production)
                || _updateOption == AutoUpdateOption.NewerThanCurrent)
            {
                return string.Format(CultureInfo.InvariantCulture,
                    Properties.Resources.VersionBarPreReleaseVersion,
                    VersionTools.GetAppVersion());
            }

            return string.Empty;
        }

        public void HandleToggleStatusChanged(bool isEnabled)
        {
            if (isEnabled)
            {
                this.CurrentView = CCAView.Automatic;
                EnableElementSelector();
            }
            else
            {
                this.CurrentView = CCAView.Manual;
                DisableElementSelector();
                HollowHighlightDriver.GetDefaultInstance().Clear();
                SelectAction.GetDefaultInstance().ClearSelectedContext();
            }
        }
    }
}
