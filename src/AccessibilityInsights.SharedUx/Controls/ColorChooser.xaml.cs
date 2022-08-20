// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.SharedUx.Telemetry;
using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace AccessibilityInsights.SharedUx.Controls
{
    public enum ColorChanger { Dropdown, Eyedropper, Text }

    public class ColorChangedEventArgs : EventArgs
    {
        public ColorChanger Source { get; set; }
    }

    /// <summary>
    /// Encapsulates an eyedropper button and a standard color picker dropdown
    ///
    /// Interaction logic for ColorChooser.xaml
    /// </summary>
    public partial class ColorChooser : UserControl
    {
        public ColorChooser()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Represents whether the stored color has been
        /// set by the user after its default initialization
        /// </summary>
        public bool ColorSetByUser { get; private set; }

        /// <summary>
        /// Handler for when one of the color-changing mechanisms has been invoked
        /// (could be eyedropper, dropdown, or hex)
        /// The sender is the specific control invoked
        /// </summary>
        public event EventHandler<ColorChangedEventArgs> ColorChangerInvoked;


        public static readonly DependencyProperty StoredColorProperty =
            DependencyProperty.Register("StoredColor", typeof(Color), typeof(ColorChooser));

        public Color StoredColor
        {
            get
            {
                return (Color)GetValue(StoredColorProperty);
            }
            set
            {
                SetValue(StoredColorProperty, value);
            }
        }

        /// <summary>
        /// Text to append to front of color picker combobox automation name
        /// </summary>
        public string ColorPickerName
        {
            get
            {
                return (string)GetValue(ColorPickerNameProperty);
            }
            set
            {
                SetValue(ColorPickerNameProperty, value);
            }
        }

        public static readonly DependencyProperty ColorPickerNameProperty =
           DependencyProperty.Register("ColorPickerName", typeof(string), typeof(ColorChooser));

        /// <summary>
        /// When the control is reset, colors are reset to the default color
        /// </summary>
        public Color DefaultColor { get; set; } = Colors.White;

        // Event fired when color picker is clicked
        private void colorPicker_Click(object sender, RoutedEventArgs e)
        {
            Logger.PublishTelemetryEvent(TelemetryAction.ColorContrast_Click_Eyedropper);
            ColorChangerInvoked?.Invoke(this, new ColorChangedEventArgs { Source = ColorChanger.Eyedropper });
        }

        /// <summary>
        /// Resets the internal color to white and
        /// the ColorSetByUser flag to false
        /// </summary>
        public void Reset()
        {
            StoredColor = DefaultColor;
            ColorSetByUser = false;
        }

        /// <summary>
        /// Call to signify that recording has completed,
        /// updates ColorSetByUser flag and saves copy of
        /// the stored color in case we need to revert later
        /// </summary>
        public void RecordingCompleted()
        {
            ColorSetByUser = true;
        }

        /// <summary>
        /// Overriding LocalizedControlType
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "group");
        }

        /// <summary>
        /// Invoke ColorChangerInvoked with text source
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void colorTb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ColorChangerInvoked?.Invoke(this, new ColorChangedEventArgs { Source = ColorChanger.Text });
            Logger.PublishTelemetryEvent(TelemetryAction.ColorContrast_Click_HexChange);
        }

        /// <summary>
        /// Send telemetry when click on color dropdown
        /// </summary>
        private void Popup_Opened(object sender, EventArgs e)
        {
            ColorChangerInvoked?.Invoke(this, new ColorChangedEventArgs() { Source = ColorChanger.Dropdown });
            Logger.PublishTelemetryEvent(TelemetryAction.ColorContrast_Click_Dropdown);
        }

        /// <summary>
        /// Close popup with Esc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Popup_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                (sender as Popup).IsOpen = false;
                e.Handled = true;
                this.cbColorPicker.Focus();
            }
        }
    }
}
