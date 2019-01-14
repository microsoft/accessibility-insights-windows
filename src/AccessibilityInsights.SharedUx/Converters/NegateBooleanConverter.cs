// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace AccessibilityInsights.SharedUx.Converters
{
    /// <summary>
    /// Converter used to bind to the negated value of a boolean
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class NegateBooleanConverter : MarkupExtension, IValueConverter
    {
        private static NegateBooleanConverter _instance;

        public NegateBooleanConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new NegateBooleanConverter());
        }
    }
}
