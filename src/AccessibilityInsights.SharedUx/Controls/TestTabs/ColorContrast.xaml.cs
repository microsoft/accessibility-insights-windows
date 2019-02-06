// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions.Contexts;
using AccessibilityInsights.Desktop.Settings;
using AccessibilityInsights.Desktop.Utility;
using AccessibilityInsights.DesktopUI.Controls;
using AccessibilityInsights.SharedUx.Dialogs;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Controls.TestTabs
{
    /// <summary>
    /// Color contrast user control
    /// 
    /// maintains two color choosers and computes color contrast between
    /// them when they are populated
    /// 
    /// maintains an eyedropper screenshot that is shared
    /// between the two color choosers
    /// </summary>
    public partial class ColorContrast : UserControl
    {
        const string HelpURL = @"https://aka.ms/accessibility-insights-colorcontrast";

        public ElementContext ElementContext { get; private set; }
        
        /// <summary>
        /// Window with screenshot that is shared between the two color choosers
        /// </summary>
        private EyedropperWindow dropper;

        /// <summary>
        /// Keeps track of whether the dropper window is currently showing
        /// on the screen. 
        /// </summary>
        private bool showingDropper = false;

        public ColorContrastViewModel ContrastVM { get; set; }

        public static readonly DependencyProperty CCAModeProperty =
            DependencyProperty.Register("CCAMode", typeof(ContrastAnalysisType), typeof(ColorContrast));

        public ContrastAnalysisType CCAMode
        {
            get
            {
                return (ContrastAnalysisType)GetValue(CCAModeProperty);
            }
            set
            {
                SetValue(CCAModeProperty, value);
            }
        }

        public ColorContrast()
        {
            InitializeComponent();
            this.ContrastVM = new ColorContrastViewModel();

            // When user interacts with color chooser, reset selected element, hide pixel locations, and 
            //  begin recording if eyedropper action is selected
            this.firstChooser.ColorChangerInvoked += (sender, colorChangerType) => {
                this.ContrastVM.Element = null;
                Chooser_ColorPickerClicked(sender, colorChangerType);
            };
            this.secondChooser.ColorChangerInvoked += (sender, colorChangerType) => {
                Chooser_ColorPickerClicked(sender, colorChangerType);
            };

            // initial colors
            this.ContrastVM.FirstColor = System.Windows.Media.Colors.Black;
            this.ContrastVM.SecondColor = System.Windows.Media.Colors.White;
        }

        /// <summary>
        /// Create the eyedropper window and link it with
        /// the current color chooser for color updates
        /// </summary>
        private void InitializeDropperWindow()
        {
            dropper = new EyedropperWindow();
            UpdateDropperImage();
            
            // Route event to current color chooser
            dropper.RecordingCompleted = () =>
            {
                CurrentChooser?.RecordingCompleted();
                CurrentChooser = null;
                ChangeColorPickingState();
            };

            // Turn highlighter button off when closed
            dropper.Closed += Dropper_Closed;
        }

        private void Dropper_Closed(object sender, EventArgs e)
        {
            // Only set the highlighter when the dropper is closed explicitly (e.g. not when switching tabs)
            if (showingDropper)
            {
                this.SetHighlighterButtonState?.Invoke(false);
            }
            ToggleModalExperience(false);
            showingDropper = false;
            CurrentChooser = null;
        }

        #region highlighter-related
        /// <summary>
        /// Set Highlighter button state in main UI
        /// this should be provided from Test Mode controller.
        /// </summary>
        public Action<bool> SetHighlighterButtonState { get; set; }
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
                if (value)
                {
                    Show();
                    Application.Current.MainWindow.Activate();
                }
                else
                {
                    Hide();
                }
            }
        }

        /// <summary>
        /// Implicitly turn on highlighter when recording starts,
        /// and show the dropper
        /// </summary>
        private void TurnOnHighlighter()
        {
            this.SetHighlighterButtonState(true);
            this.HighlightVisibility = true;
        }
        #endregion

        private ColorChooser currentChooser;
        /// <summary>
        /// Reference to the current color chooser in use
        /// Makes border visible for current chooser
        /// </summary>
        private ColorChooser CurrentChooser
        {
            get
            {
                return currentChooser;
            }
            set
            {
                var oldChooser = currentChooser;
                currentChooser = value;
                if (oldChooser != null)
                {
                    oldChooser.BorderBrush = System.Windows.Media.Brushes.Transparent;
                }
                if (currentChooser != null)
                {
                    currentChooser.BorderBrush = System.Windows.Media.Brushes.Black;
                }
            }
        }

        /// <summary>
        /// For convenience, describes whether we are currently recording the first color or second color
        /// </summary>
        private bool SelectingFirstColor => currentChooser == firstChooser;
        private bool SelectingSecondColor => currentChooser == secondChooser;

        /// <summary>
        /// Sets element context and updates UI
        /// </summary>
        /// <param name="ec"></param>
        public void SetElement(ElementContext ec)
        {
            this.ElementContext = ec;
            UpdateDropperImage();
            if (this.IsVisible)
            {
                TurnOnHighlighter();
            }
        }

        /// <summary>
        /// Update the eyedropper UI with current screenshot from ElementContext
        /// Also shows selected pixels if set in ContrastVM
        /// </summary>
        private void UpdateDropperImage()
        {
            if (ElementContext?.DataContext?.Screenshot != null)
            {
                var dpi = AccessibilityInsights.Desktop.Utility.ExtensionMethods.GetDPI((int)Application.Current.MainWindow.Top, (int)Application.Current.MainWindow.Left);
                var window = ElementContext.DataContext.Elements[ElementContext.DataContext.ScreenshotElementId];
                var dim = window.BoundingRectangle;

                dropper?.UpdateBackground(ElementContext);
                dropper.Top = dim.Top / dpi;
                dropper.Left = dim.Left / dpi;

                if (ContrastVM.FirstPixel != null)
                {
                    this.dropper.SetFirstColorLocation(ContrastVM.FirstPixel.Value);
                }
                if (ContrastVM.SecondPixel != null)
                {
                    this.dropper.SetSecondColorLocation(ContrastVM.SecondPixel.Value);
                }
                this.dropper.ShowFirstColorLocation(ContrastVM.FirstPixel != null);
                this.dropper.ShowSecondColorLocation(ContrastVM.SecondPixel != null);
            }
        }

        /// <summary>
        /// When either chooser's color picker is clicked,
        /// we begin eyedropper color recording and set
        /// its target as the sender
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Chooser_ColorPickerClicked(object sender, SourceArgs e)
        {
            this.ContrastVM.BugId = null;
            if (CCAMode == ContrastAnalysisType.Test 
                && e.Source == ColorChanger.Eyedropper 
                && this.ElementContext!= null
                && this.ElementContext.DataContext!=null
                && this.ElementContext.DataContext.Screenshot!=null)
            {
                TurnOnHighlighter();
                BeginRecording(sender as ColorChooser);
                if (SelectingFirstColor)
                {
                    dropper?.ShowFirstColorLocation(true);
                }
                else if (SelectingSecondColor)
                {
                    dropper?.ShowSecondColorLocation(true);
                }
            }
            else if (CCAMode == ContrastAnalysisType.Live && e.Source == ColorChanger.Eyedropper)
            {
                CurrentChooser = sender as ColorChooser;
                var cc = new GlobalEyedropperWindow(ContrastVM, SelectingFirstColor, SelectingSecondColor, Dropper_Closed) { Owner = Application.Current.MainWindow };
                cc.Show();
            }
        }

        /// <summary>
        /// Begin eyedropper recording to the specified target
        /// </summary>
        /// <param name="target">ColorChooser that is in use</param>
        private void BeginRecording(ColorChooser target)
        {
            CurrentChooser = target;
            var initialMagnificationPoint = SelectingFirstColor ? this.ContrastVM.FirstPixel : this.ContrastVM.SecondPixel;
            dropper?.StartRecording(initialMagnificationPoint ?? System.Drawing.Point.Empty, 
                pixelLocationChanged: (point) => {
                    var color = this.ElementContext.DataContext.Screenshot.GetPixel(point.X, point.Y);
                    var castedColor = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
                    if (SelectingFirstColor)
                    {
                        this.ContrastVM.FirstColor = castedColor;
                        this.ContrastVM.FirstPixel = point;
                        dropper?.SetFirstColorLocation(point);

                        // translate the point by parent window location so we can compare
                        // it to bounding rectangles of elements in that window
                        var allElements = this.ElementContext.DataContext.Elements;
                        var parentWindow = this.ElementContext.DataContext.Element.GetParentWindow();
                        point.Offset(parentWindow.BoundingRectangle.Location.X, parentWindow.BoundingRectangle.Location.Y);
                        this.ContrastVM.Element = AccessibilityInsights.Actions.Misc.ExtensionMethods.GetSmallestElementFromPoint(allElements, point);
                    }
                    else if (SelectingSecondColor)
                    {
                        this.ContrastVM.SecondColor = castedColor;
                        this.ContrastVM.SecondPixel = point;
                        dropper?.SetSecondColorLocation(point);
                    }
                }
            );
            ToggleModalExperience(true);
        }

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
        /// Hide the existing highlight action window
        /// Recreate if needed and show the dropper window if tab is visible/highlighter-on
        /// Set appropriate UI state according to whether
        ///     colors have been chosen by the user
        /// </summary>
        private void Show()
        {
            Actions.HighlightImageAction.GetDefaultInstance().Hide();

            if (CCAMode == ContrastAnalysisType.Test && !showingDropper && this.IsVisible && this.HighlightVisibility && this.ElementContext != null)
            {
                InitializeDropperWindow();
                showingDropper = true;
                dropper.Show();

                // Feedback from PM is that we may have guided two-step experience in future but
                // for now it is not guided and the user clicks the picker button to start off.
                // This means we are always going to be in FreeFormSelection state.
                this.CurrentState = PickingState.FreeFormSelection;
                //this.CurrentState = !firstChooser.ColorSetByUser && !secondChooser.ColorSetByUser ?
                //    PickingState.BeforePickingAnyColors : PickingState.FreeFormSelection;

                ChangeColorPickingState();
            }
        }

        /// <summary>
        /// Close the eyedropper window
        /// </summary>
        private void Hide()
        {
            showingDropper = false;
            this.dropper?.Close();
        }

        /// <summary>
        /// Enables/disables the color choosers
        /// when recording in the eyedropper has begun or ended
        /// </summary>
        /// <param name="start">true if recording has begun (should appear modal), false otherwise</param>
        private void ToggleModalExperience(bool start)
        {
            this.firstChooser.IsEnabled = !start;
            this.secondChooser.IsEnabled = !start;
        }

        /// <summary>
        /// Reset color choosers to initial values and close eyedropper window
        /// </summary>
        public void ClearUI()
        {
            this.ElementContext = null;
            this.ContrastVM.Reset();
            this.firstChooser.Reset();
            this.secondChooser.Reset();
            Hide();
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
        /// Overriding LocalizedControlType
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "pane");
        }

        /// <summary>
        /// When a file is saved from this mode, the resulting dictionary contains meta
        /// information useful for loading that isn't saved in the data context itself
        /// In this case, the resulting dictionary holds the existing color values so they can be shown upon load
        /// </summary>
        /// <returns>Dictionary with current color values</returns>
        public Dictionary<SnapshotMetaPropertyName, object> GetMetaPropertiesToSave()
        {
            Dictionary<SnapshotMetaPropertyName, object> info = new Dictionary<SnapshotMetaPropertyName, object>();
            info[SnapshotMetaPropertyName.FirstColor] = this.ContrastVM.FirstColor;
            info[SnapshotMetaPropertyName.SecondColor] = this.ContrastVM.SecondColor;
            info[SnapshotMetaPropertyName.FirstPixel] = this.ContrastVM.FirstPixel;
            info[SnapshotMetaPropertyName.SecondPixel] = this.ContrastVM.SecondPixel;
            return info;
        }

        private PickingState CurrentState;
        /// <summary>
        /// State that helps manage the initial workflow. Users start in 
        /// "BeforePickingAnyColors" and must pick 2 colors in succession before ending
        /// up in "FreeFormSelection". Once the user is in "FreeFormSelection",
        /// they never leave that state internally.
        /// </summary>
        private enum PickingState
        {
            BeforePickingAnyColors, // first time in color contrast, precursor to forced two-color selection
            PickingFirstColor,
            PickingSecondColor,
            FreeFormSelection // colors have been selected before
        }

        /// <summary>
        /// There is a scenario where the user must pick two colors in succession
        /// instead of explicitly clicking the eyedropper button. This method
        /// keeps track of what state/step the user is in and 
        /// changes the UI so it is blocked & in color-picking mode
        /// </summary>
        private void ChangeColorPickingState()
        {
            switch (this.CurrentState)
            {
                case PickingState.BeforePickingAnyColors:
                    this.CurrentState = PickingState.PickingFirstColor;
                    BeginRecording(firstChooser);
                    break;
                case PickingState.PickingFirstColor:
                    this.CurrentState = PickingState.PickingSecondColor;
                    BeginRecording(secondChooser);
                    break;
                case PickingState.PickingSecondColor:
                case PickingState.FreeFormSelection:
                    ToggleModalExperience(false);
                    this.CurrentState = PickingState.FreeFormSelection;
                    break;
            }
        }

        /// <summary>
        /// Open the How to test link
        /// </summary>
        private void hlHowToTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(HelpURL));
            }
            catch
            {
                MessageDialog.Show(string.Format(CultureInfo.InvariantCulture, Properties.Resources.ColorContrast_hlHowToTest_Click, HelpURL));
            }
        }

        private void TgbtnAutoDetect_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
