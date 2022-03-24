// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Properties;
using AccessibilityInsights.SharedUx.ViewModels;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace AccessibilityInsights.SharedUx.Converters
{
    [ValueConversion(typeof(BaseActionViewModel), typeof(UserControl))]
    public class ActionViewModelConverter : MarkupExtension, IValueConverter
    {
        private static ActionViewModelConverter _instance;

        public ActionViewModelConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var attrs = value.GetType().GetCustomAttributes(typeof(TargetActionViewAttribute), false);

            if (attrs != null && attrs.Length == 1)
            {
                var attr = (TargetActionViewAttribute)attrs[0];

                return Activator.CreateInstance(attr.ViewType, value);
            }

            throw new ArgumentException(Resources.ActionViewModelConverter_Convert_passed_value_is_not_supported_type);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { // read only converter...
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new ActionViewModelConverter());
        }
    }
}
