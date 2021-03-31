// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Properties;
using AccessibilityInsights.SharedUx.Telemetry;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace AccessibilityInsights.SharedUx.Converters
{
    /// <summary>
    /// Converts between the hex string value of a color and its representation as
    /// a System.Windows.Media.Color. For example, #FFFFFF is White.
    /// </summary>
    [ValueConversion(typeof(System.Windows.Media.Color), typeof(String))]
    public class ColorStringConverter : MarkupExtension, IValueConverter
    {
        private static ColorStringConverter _instance;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString((string)value);

            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                return System.Windows.Media.Colors.White;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Windows.Media.Color colorValue = (System.Windows.Media.Color) value;
            return String.Format(CultureInfo.InvariantCulture, Resources.ColorStringConverter_Convert_0_X2_1_X2_2_X2, colorValue.R, colorValue.G, colorValue.B);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new ColorStringConverter());
        }
    }
}
