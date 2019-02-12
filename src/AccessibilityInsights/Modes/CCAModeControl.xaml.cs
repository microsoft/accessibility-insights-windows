// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.Actions;
using System.Windows.Automation.Peers;
using AccessibilityInsights.SharedUx.Controls.CustomControls;
using AccessibilityInsights.Actions.Contexts;
using AccessibilityInsights.Enums;
using AccessibilityInsights.DesktopUI.Enums;
using System.Windows.Threading;
using System.Globalization;
using AccessibilityInsights.Actions.Enums;
using AccessibilityInsights.Actions.Misc;
using AccessibilityInsights.RulesTest;

namespace AccessibilityInsights.Modes
{
    /// <summary>
    /// Interaction logic for CCAModeControl.xaml
    /// </summary>
    public partial class CCAModeControl : UserControl, IModeControl
    {

        /// <summary>
        /// Indicate how to do the data context population. 
        /// Live/Snapshot/Load
        /// </summary>
        public DataContextMode DataContextMode { get; set; } = DataContextMode.Test;

        /// <summary>
        /// MainWindow to access shared methods
        /// </summary>
        static MainWindow MainWin
        {
            get
            {
                return (MainWindow)Application.Current.MainWindow;
            }
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
        /// Layout configuration
        /// </summary>
        public static AppLayout CurrentLayout
        {
            get
            {
                return ConfigurationManager.GetDefaultInstance()?.AppLayout;
            }
        }

        /// <summary>
        /// Overriding LocalizedControlType
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "page");
        }

        public CCAModeControl()
        {
            InitializeComponent();
        }

        // <summary>
        // Updates Window size with stored data
        // </summary>
        public void AdjustMainWindowSize()
        {
            MainWin.SizeToContent = SizeToContent.Manual;
        }

        ///not implemented--nothing will copy
        public void CopyToClipboard()
        {
            return;
        }

        /// <summary>
        /// Hide control and hilighter
        /// </summary>
        public void HideControl()
        {
            UpdateConfigWithSize();
            this.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Show control and hilighter
        /// </summary>
        public void ShowControl()
        {
            this.Visibility = Visibility.Visible;
            HighlightAction.GetDefaultInstance().HighlighterMode = HighlighterMode.Highlighter;

            HighlightAction.GetDefaultInstance().Clear();

            SelectAction.GetDefaultInstance().ClearSelectedContext();

            Dispatcher.InvokeAsync(() =>
            {
                this.SetFocusOnDefaultControl();
            }
            , System.Windows.Threading.DispatcherPriority.Input);
        }

        /// <summary>
        /// set element
        /// </summary>
        /// <param name="ecId"></param>
        public async Task SetElement(Guid ecId)
        {
            if (GetDataAction.ExistElementContext(ecId))
            {
                try
                {
                    HighlightAction.GetDefaultInstance().HighlighterMode = HighlighterMode.Highlighter;

                    HighlightAction.GetDefaultInstance().SetElement(ecId, 0);

                    ElementContext ec = null;
                    string warning = string.Empty;
                    string toolTipText = string.Empty;

                    await Task.Run(() =>
                    {
                        var updated = CaptureAction.SetTestModeDataContext(ecId, this.DataContextMode, Configuration.TreeViewMode);
                        ec = GetDataAction.GetElementContext(ecId);

                        // send telemetry of scan results.
                        var dc = GetDataAction.GetElementDataContext(ecId);
                        if (dc.ElementCounter.UpperBoundExceeded)
                        {
                            warning = string.Format(CultureInfo.InvariantCulture,
                                Properties.Resources.SetElementCultureInfoFormatMessage,
                                dc.ElementCounter.UpperBound);
                        }
                        dc.PublishScanResults();
                    }).ConfigureAwait(false);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (ec == null || ec.Element == null)
                        {
                            toolTipText = "No Eelement Selected!";
                        }
                        else
                        {

                            if (CCAControlTypesFilter.GetDefaultInstance().Contains(ec.Element.ControlTypeId))
                            {
                                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                                {
                                    this.ctrlContrast.SetElement(ec);
                                })).Wait();
                                //toolTipText = string.Format(CultureInfo.InvariantCulture, "Ratio: {0}\nConfidence: {1}\n",
                                  //  this.ctrlContrast.output.Text, this.ctrlContrast.tbConfidence.Text);
                            }
                            else
                            {
                                toolTipText = "Unknown Element Type!";
                            }
                        }

                        MainWin.CurrentView = CCAView.Automatic;

                        HighlightAction.GetDefaultInstance().HighlighterMode = HighlighterMode.HighlighterTooltip;


                        HighlightAction.GetDefaultInstance().SetText(toolTipText);

                        // enable element selector
                        MainWin.EnableElementSelector();
                    });


                }
                catch (Exception)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MainWin.CurrentView = CCAView.Automatic;
                        // enable element selector
                        MainWin.EnableElementSelector();
                    });
                }

            }

        }


        /*
        /// <summary>
        /// Will be replaced by Dequeue API
        /// </summary>
        /// <returns></returns>
        private string getScreenShotRatio(Bitmap screenshot)
        {
            return "Ratio: 1:1\nConfiedency: Excellent";
        }*/

        /// <summary>
        /// Make sure that statemachine and UI are updated for Live mode. 
        /// </summary>
        private static void UpdateStateMachineForCCAAutomaticMode()
        {
            // enable selector once UI update is finished. 
            MainWin?.SetCurrentViewAndUpdateUI(CCAView.Automatic);
            MainWin?.EnableElementSelector();
        }
#pragma warning restore CS1998

        /// <summary>
        /// Not needed
        /// </summary>
        public void UpdateConfigWithSize()
        {
        }

        public void Clear()
        {
        }

        /// <summary>
        /// Refresh button is not needed on main command bar
        /// </summary>
        public bool IsRefreshEnabled { get { return false; } }

        /// <summary>
        /// Save button is not neeeded on main command bar
        /// </summary>
        public bool IsSaveEnabled { get { return false; } }

        /// <summary>
        /// No action
        /// </summary>
        public void Refresh()
        {
        }

        /// <summary>
        /// No action
        /// </summary>
        public void Save()
        {
        }

        /// <summary>
        /// Handle toggle highlighter request
        /// </summary>
        /// <returns></returns>
        public bool ToggleHighlighter()
        {
            var enabled = !HighlightAction.GetDefaultInstance().IsEnabled;
            HighlightAction.GetDefaultInstance().IsEnabled = enabled;
            return enabled;
        }

        /// <summary>
        /// Set focus on default control for mode
        /// </summary>
        public void SetFocusOnDefaultControl()
        {
            this.ctrlContrast.Focus();
        }
    }
}
