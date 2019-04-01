// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions;
using AccessibilityInsights.Actions.Contexts;
using AccessibilityInsights.Actions.Enums;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.DesktopUI.Enums;
using AccessibilityInsights.Enums;
using AccessibilityInsights.Rules;
using AccessibilityInsights.SharedUx.Controls.CustomControls;
using AccessibilityInsights.SharedUx.Interfaces;
using AccessibilityInsights.SharedUx.Settings;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Threading;

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
        public DataContextMode DataContextMode { get; set; } = DataContextMode.Live;

        private HighlighterMode prevMode;
        private bool prevHighlighterState;

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

        public static void SetCurrentCCAView(bool isToogleOn)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (isToogleOn)
                {
                    MainWin.CurrentView = CCAView.Automatic;
                }
                else
                {
                    MainWin.CurrentView = CCAView.Manual;
                }
            });
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
            HighlightAction.GetDefaultInstance().HighlighterMode = prevMode;
            MainWin.SetHighlightBtnState(prevHighlighterState);
            HighlightAction.GetDefaultInstance().IsEnabled = prevHighlighterState;
            UpdateConfigWithSize();
            this.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Show control and hilighter
        /// </summary>
        public void ShowControl()
        {
            this.Visibility = Visibility.Visible;
            this.prevHighlighterState = HighlightAction.GetDefaultInstance().IsEnabled;
            this.prevMode = HighlightAction.GetDefaultInstance().HighlighterMode;
            HighlightAction.GetDefaultInstance().HighlighterMode = HighlighterMode.Highlighter;
            MainWin.SetHighlightBtnState(true);
            HighlightAction.GetDefaultInstance().IsEnabled = true;
            HighlightAction.GetDefaultInstance().Clear();

            SelectAction.GetDefaultInstance().ClearSelectedContext();

            Dispatcher.InvokeAsync(() =>
            {
                this.SetFocusOnDefaultControl();
            }
            , System.Windows.Threading.DispatcherPriority.Input);

            ctrlContrast.SetAutoCCAState(true);
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

                    this.ctrlContrast.ActivateProgressRing();

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
                                toolTipText = string.Format(CultureInfo.InvariantCulture, "Ratio: {0}\nConfidence: {1}",
                                    this.ctrlContrast.getRatio(), this.ctrlContrast.getConfidence());
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

                    this.ctrlContrast.DeactivateProgressRing();
                }
                catch (Exception ex)
                {
                    ex.ReportException();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MainWin.CurrentView = CCAView.Automatic;
                        HighlightAction.GetDefaultInstance().HighlighterMode = HighlighterMode.HighlighterTooltip;


                        HighlightAction.GetDefaultInstance().SetText("Unable to detect colors!");
                        // enable element selector
                        MainWin.EnableElementSelector();

                        this.ctrlContrast.DeactivateProgressRing();
                    });
                }

            }

        }

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

        public bool isToggleChecked()
        {
            return this.ctrlContrast.IsToggleChecked();
        }
    }
}
