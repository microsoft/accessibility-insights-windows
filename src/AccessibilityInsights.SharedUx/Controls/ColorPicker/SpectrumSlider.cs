// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AccessibilityInsights.SharedUx.Controls.ColorPicker
{
    /// <summary>
    /// Credit to: https://blogs.msdn.microsoft.com/wpfsdk/2006/10/26/uncommon-dialogs-font-chooser-color-picker-dialogs/
    /// </summary>
    public class SpectrumSlider : Slider
    {
        static SpectrumSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpectrumSlider),
                new FrameworkPropertyMetadata(typeof(SpectrumSlider)));
        }

        #region Public Properties
        public Color SelectedColor
        {
            get
            {
                return (Color)GetValue(SelectedColorProperty);
            }
            set
            {
                SetValue(SelectedColorProperty, value);
            }
        }
        #endregion

        #region Dependency Property Fields
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register
            ("SelectedColor", typeof(Color), typeof(SpectrumSlider),
            new PropertyMetadata(System.Windows.Media.Colors.Transparent));

        #endregion

        #region Public Methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_spectrumDisplay = GetTemplateChild(SpectrumDisplayName) as Rectangle;
            updateColorSpectrum();
            OnValueChanged(Double.NaN, Value);
        }
        #endregion

        #region Protected Methods
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            Color theColor = ColorUtilities.ConvertHsvToRgb(newValue, 1, 1);
            SetValue(SelectedColorProperty, theColor);
        }
        #endregion

        #region Private Methods
        private void updateColorSpectrum()
        {
            if (m_spectrumDisplay != null)
            {
                createSpectrum();
            }
        }

        private void createSpectrum()
        {
            pickerBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0, .5),
                EndPoint = new Point(1, .5),
                ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation
            };

            List<Color> colorsList = ColorUtilities.GenerateHsvSpectrum();
            double stopIncrement = (double)1 / colorsList.Count;

            int i;
            for (i = 0; i < colorsList.Count; i++)
            {
                pickerBrush.GradientStops.Add(new GradientStop(colorsList[i], i * stopIncrement));
            }

            pickerBrush.GradientStops[i - 1].Offset = 1.0;
            m_spectrumDisplay.Fill = pickerBrush;
        }
        #endregion

        #region Private Fields
        private const string SpectrumDisplayName = "PART_SpectrumDisplay";
        private Rectangle m_spectrumDisplay;
        private LinearGradientBrush pickerBrush;
        #endregion
    }
}
