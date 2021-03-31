// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.ViewModels;
using Axe.Windows.Core.Bases;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace AccessibilityInsights.SharedUx.Converters
{
    [ValueConversion(typeof(A11yProperty), typeof(PropertyListViewItemModel))]
    public class A11yPropertyConverter : MarkupExtension, IValueConverter
    {
        private static A11yPropertyConverter _instance;

        public A11yPropertyConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new PropertyListViewItemModel((A11yProperty)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { // read only converter...
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new A11yPropertyConverter());
        }
    }
}
