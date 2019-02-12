// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions.Contexts;
using AccessibilityInsights.Desktop.ColorContrastAnalyzer;
using AccessibilityInsights.Desktop.Utility;
using AccessibilityInsights.DesktopUI.Controls;
using AccessibilityInsights.SharedUx.Dialogs;
using AccessibilityInsights.SharedUx.Interfaces;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.ViewModels;
using System;
using System.Diagnostics;
using System.Drawing;
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

        public ColorContrastViewModel ContrastVM { get; set; }

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

        private void Dropper_Closed(object sender, EventArgs e)
        {
            ToggleModalExperience(false);
            CurrentChooser = null;
        }

        /// <summary>
        /// Set Highlighter button state in main UI
        /// this should be provided from Test Mode controller.
        /// </summary>
        public Action<bool> SetHighlighterButtonState { get; set; }

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
            var bm = ec.Element.CaptureBitmap();
            RunAutoCCA(bm);
        }

        private void RunAutoCCA(Bitmap bitmap)
        {
            var bmc = new BitmapCollection(bitmap);
            var result = bmc.RunColorContrastCalculation();
            var pair = result.GetMostLikelyColorPair();
            this.ContrastVM.FirstColor = pair.DarkerColor.MediaColor;
            this.ContrastVM.SecondColor = pair.LighterColor.MediaColor;
            tbConfidence.Text = result.ConfidenceValue().ToString();
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
            if (e.Source == ColorChanger.Eyedropper)
            {
                CurrentChooser = sender as ColorChooser;
                var cc = new GlobalEyedropperWindow(ContrastVM, SelectingFirstColor, SelectingSecondColor, Dropper_Closed) { Owner = Application.Current.MainWindow };
                cc.Show();
            }
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
        }

        public object getConfidence()
        {
            return this.tbConfidence.Text;
        }

        public object getRatio()
        {
            return this.output.Text;
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
            IMainWindow wnd = Application.Current.MainWindow as IMainWindow;
            ICCAMode ch = Application.Current.MainWindow as ICCAMode;

            // Watson crashes suggest that this will be null sometimes
            if (wnd != null)
            {
                if (this.tgbtnAutoDetect.IsChecked.HasValue)
                {
                    ch.HandleToggleStatusChanged(this.tgbtnAutoDetect.IsChecked.Value);
                }
            }
        }

        public bool isToggleChecked()
        {
            return this.tgbtnAutoDetect.IsChecked.Value;
        }

        public void ActivateProgressRing(){
            this.ctrlProgressRing.Activate();
        }

        public void DeactivateProgressRing()
        {
            this.ctrlProgressRing.Deactivate();
        }

    }
}
