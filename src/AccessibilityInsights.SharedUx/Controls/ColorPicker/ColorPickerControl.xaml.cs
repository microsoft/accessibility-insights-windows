// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static AccessibilityInsights.SharedUx.Controls.ColorPicker.ColorUtilities;

namespace AccessibilityInsights.SharedUx.Controls.ColorPicker
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// https://blogs.msdn.microsoft.com/wpfsdk/2006/10/26/uncommon-dialogs-font-chooser-color-picker-dialogs/
    /// </summary>
    public partial class ColorPickerControl : UserControl
    {
        public ColorPickerControl()
        {
            InitializeComponent();
            templateApplied = false;
            m_color = Colors.White;
            shouldFindPoint = true;

            SetValue(RedProperty, m_color.R);
            SetValue(GreenProperty, m_color.G);
            SetValue(BlueProperty, m_color.B);
            SetValue(SelectedColorProperty, m_color);

            sliderColorSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(BaseColorChanged);

            tmbColorMarker.RenderTransform = markerTransform;
            tmbColorMarker.RenderTransformOrigin = new Point(0.5, 0.5);
            brdrColorDetail.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
            brdrColorDetail.PreviewMouseMove += new MouseEventHandler(OnMouseMove);
            brdrColorDetail.SizeChanged += new SizeChangedEventHandler(ColorDetailSizeChanged);

            templateApplied = true;
            shouldFindPoint = true;
        }

        /// <summary>
        /// Call control a pane
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "pane");
        }

        #region Public Properties

        // Gets or sets the selected color.
        public Color SelectedColor
        {
            get
            {
                return (Color)GetValue(SelectedColorProperty);
            }
            set
            {
                SetValue(SelectedColorProperty, m_color);
                setColor(value);
            }
        }

        #region RGB Properties
        // Gets or sets the ARGB red value of the selected color.
        public byte Red
        {
            get
            {
                return (byte)GetValue(RedProperty);
            }
            set
            {
                SetValue(RedProperty, value);
            }
        }

        // Gets or sets the ARGB green value of the selected color.
        public byte Green
        {
            get
            {
                return (byte)GetValue(GreenProperty);
            }
            set
            {
                SetValue(GreenProperty, value);
            }
        }

        // Gets or sets the ARGB blue value of the selected color.
        public byte Blue
        {
            get
            {
                return (byte)GetValue(BlueProperty);
            }
            set
            {
                SetValue(BlueProperty, value);
            }
        }
        #endregion RGB Properties

        // Gets or sets the selected color in hexadecimal notation.
        public string HexadecimalString
        {
            get
            {
                return (string)GetValue(HexadecimalStringProperty);
            }
            set
            {
                SetValue(HexadecimalStringProperty, value);
            }
        }

        #endregion

        #region Public Events

        public event RoutedPropertyChangedEventHandler<Color> SelectedColorChanged
        {
            add
            {
                AddHandler(SelectedColorChangedEvent, value);
            }

            remove
            {
                RemoveHandler(SelectedColorChangedEvent, value);
            }
        }

        #endregion

        #region Dependency Property Fields
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register
            (nameof(SelectedColor), typeof(Color), typeof(ColorPickerControl),
            new PropertyMetadata(Colors.Transparent,
                new PropertyChangedCallback(selectedColor_changed)

            ));

        public static readonly DependencyProperty RedProperty =
            DependencyProperty.Register
            (nameof(Red), typeof(byte), typeof(ColorPickerControl),
            new PropertyMetadata((byte)255,
            new PropertyChangedCallback(RedChanged)
            ));

        public static readonly DependencyProperty GreenProperty =
            DependencyProperty.Register
            (nameof(Green), typeof(byte), typeof(ColorPickerControl),
            new PropertyMetadata((byte)255,
            new PropertyChangedCallback(GreenChanged)
            ));

        public static readonly DependencyProperty BlueProperty =
            DependencyProperty.Register
            (nameof(Blue), typeof(byte), typeof(ColorPickerControl),
            new PropertyMetadata((byte)255,
            new PropertyChangedCallback(BlueChanged)
            ));

        public static readonly DependencyProperty HexadecimalStringProperty =
            DependencyProperty.Register
            (nameof(HexadecimalString), typeof(string), typeof(ColorPickerControl),
            new PropertyMetadata(String.Empty,
            new PropertyChangedCallback(HexadecimalStringChanged)
         ));

        #endregion

        #region RoutedEvent Fields

        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(SelectedColorChanged),
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<Color>),
            typeof(ColorPickerControl)
        );
        #endregion

        #region Property Changed Callbacks

        private static void RedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (ColorPickerControl)d;
            c.OnRedChanged((byte)e.NewValue);
        }

        protected virtual void OnRedChanged(byte newValue)
        {
            m_color.R = newValue;
            SetValue(SelectedColorProperty, m_color);
            updateMarkerPosition(m_color);
        }

        private static void GreenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (ColorPickerControl)d;
            c.OnGreenChanged((byte)e.NewValue);
        }

        protected virtual void OnGreenChanged(byte newValue)
        {
            m_color.G = newValue;
            SetValue(SelectedColorProperty, m_color);
            updateMarkerPosition(m_color);
        }

        private static void BlueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (ColorPickerControl)d;
            c.OnBlueChanged((byte)e.NewValue);
        }

        protected virtual void OnBlueChanged(byte newValue)
        {
            m_color.B = newValue;
            SetValue(SelectedColorProperty, m_color);
            updateMarkerPosition(m_color);
        }

        private static void HexadecimalStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (ColorPickerControl)d;
            c.OnHexadecimalStringChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void OnHexadecimalStringChanged(string oldValue, string newValue)
        {
            try
            {
                if (shouldFindPoint)
                {
                    m_color = (Color)ColorConverter.ConvertFromString(newValue);
                }

                SetValue(RedProperty, m_color.R);
                SetValue(GreenProperty, m_color.G);
                SetValue(BlueProperty, m_color.B);

                if (shouldFindPoint && templateApplied)
                {
                    updateMarkerPosition(m_color);
                }
            }
            catch (FormatException)
            {
                // Don't report this Exception, since it's an effect of invalid user input
                SetValue(HexadecimalStringProperty, oldValue);
            }
        }

        private static void selectedColor_changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cPicker = (ColorPickerControl)d;
            cPicker.OnSelectedColorChanged((Color)e.OldValue, (Color)e.NewValue);
        }

        protected virtual void OnSelectedColorChanged(Color oldColor, Color newColor)
        {
            RoutedPropertyChangedEventArgs<Color> newEventArgs =
                new RoutedPropertyChangedEventArgs<Color>(oldColor, newColor)
                {
                    RoutedEvent = ColorPickerControl.SelectedColorChangedEvent
                };
            RaiseEvent(newEventArgs);
            if (this.DataContext is ColorChooser cc)
            {
                cc.StoredColor = newColor;
            }
        }

        #endregion

        #region Template Part Event Handlers
        private void BaseColorChanged(object sender, RoutedPropertyChangedEventArgs<Double> e)
        {
            if (m_ColorPosition != null)
            {
                determineColor((Point)m_ColorPosition);
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(brdrColorDetail);
            updateMarkerPosition(p);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(brdrColorDetail);
                updateMarkerPosition(p);
                Mouse.Synchronize();
            }
        }

        private void ColorDetailSizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (args.PreviousSize != Size.Empty && args.PreviousSize.Width != 0 && args.PreviousSize.Height != 0)
            {
                double widthDifference = args.NewSize.Width / args.PreviousSize.Width;
                double heightDifference = args.NewSize.Height / args.PreviousSize.Height;
                markerTransform.X *= widthDifference;
                markerTransform.Y *= heightDifference;
            }
            else if (m_ColorPosition != null)
            {
                markerTransform.X = ((Point)m_ColorPosition).X * args.NewSize.Width;
                markerTransform.Y = ((Point)m_ColorPosition).Y * args.NewSize.Height;
            }
        }

        #endregion

        #region Color Resolution Helpers

        private void setColor(Color theColor)
        {
            m_color = theColor;

            if (templateApplied)
            {
                SetValue(RedProperty, m_color.R);
                SetValue(GreenProperty, m_color.G);
                SetValue(BlueProperty, m_color.B);
                updateMarkerPosition(theColor);
            }
        }

        private void updateMarkerPosition(Point p)
        {
            if (p.X < 0)
            {
                p.X = 0;
            }
            else if (p.X > brdrColorDetail.Width)
            {
                p.X = brdrColorDetail.Width;
            }

            markerTransform.X = p.X;
            p.X /= brdrColorDetail.ActualWidth;

            if (p.Y < 0)
            {
                p.Y = 0;
            }
            else if (p.Y > brdrColorDetail.Height)
            {
                p.Y = brdrColorDetail.Height;
            }

            markerTransform.Y = p.Y;
            p.Y /= brdrColorDetail.ActualHeight;

            m_ColorPosition = p;
            determineColor(p);
        }

        private void updateMarkerPosition(Color theColor)
        {
            m_ColorPosition = null;

            HsvColor hsv = ColorUtilities.ConvertRgbToHsv(theColor.R, theColor.G, theColor.B);
            sliderColorSlider.Value = 360 - hsv.H;

            Point p = new Point(hsv.S, 1 - hsv.V);

            m_ColorPosition = p;
            p.X *= brdrColorDetail.ActualWidth;
            p.Y *= brdrColorDetail.ActualHeight;
            markerTransform.X = p.X;
            markerTransform.Y = p.Y;
        }

        private void determineColor(Point p)
        {
            HsvColor hsv = new HsvColor(sliderColorSlider.Value, 1, 1)
            {
                S = p.X,
                V = 1 - p.Y
            };
            m_color = ColorUtilities.ConvertHsvToRgb(hsv.H, hsv.S, hsv.V);
            shouldFindPoint = false;
            SetValue(HexadecimalStringProperty, m_color.ToString(CultureInfo.InvariantCulture));
            shouldFindPoint = true;
        }

        #endregion

        #region Private Fields
        private readonly TranslateTransform markerTransform = new TranslateTransform();
        private Point? m_ColorPosition;
        private Color m_color;
        private bool shouldFindPoint;
        private readonly bool templateApplied;
        #endregion

        /// <summary>
        /// Allow keyboard nav of color picker
        /// </summary>
        private void tmbColorMarker_KeyDown(object sender, KeyEventArgs e)
        {
            int dx = 0;
            int dy = 0;

            switch (e.Key)
            {
                case Key.Left:
                    dx = -1;
                    break;
                case Key.Right:
                    dx = 1;
                    break;
                case Key.Up:
                    dy = -1;
                    break;
                case Key.Down:
                    dy = 1;
                    break;
            }

            if (dx != dy)
            {
                updateMarkerPosition(new Point(markerTransform.X + dx, markerTransform.Y + dy));
                e.Handled = true;
            }
        }

#pragma warning disable IDE0051 // This is referenced in the XAML file
        /// <summary>
        /// Update color and focus when control made visible
        /// </summary>
        private void tmbColorMarker_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                (sender as UIElement)?.Focus();
                if (this.DataContext is ColorChooser cc)
                    SelectedColor = cc.StoredColor;

                HexadecimalString = SelectedColor.ToString(CultureInfo.InvariantCulture);
                this.updateMarkerPosition(SelectedColor);
                m_color = SelectedColor;
            }
        }
#pragma warning restore IDE0051 // This is referenced in the XAML file
    }
}
