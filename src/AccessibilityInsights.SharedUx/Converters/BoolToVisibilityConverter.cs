// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace AccessibilityInsights.SharedUx.Converters
{
    /// <summary>
    /// Converts from a boolean to either visible or collapsed
    /// </summary>
    [ValueConversion(typeof(bool), typeof(System.Windows.Visibility))]
    public class BoolToVisibilityConverter : MarkupExtension, IValueConverter
    {
        private static BoolToVisibilityConverter _instance;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (System.Windows.Visibility)value == System.Windows.Visibility.Visible;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return (bool)value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new BoolToVisibilityConverter());
        }
    }
}
