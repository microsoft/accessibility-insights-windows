// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Properties;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace AccessibilityInsights.SharedUx.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class CheckStateConverter : MarkupExtension, IValueConverter
    {
        private static CheckStateConverter _instance;

        public CheckStateConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Resources.CheckStateConverter_Convert_Currently_checked : Resources.CheckStateConverter_Convert_Currently_unchecked;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { // read only converter...
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new CheckStateConverter());
        }
    }
}
