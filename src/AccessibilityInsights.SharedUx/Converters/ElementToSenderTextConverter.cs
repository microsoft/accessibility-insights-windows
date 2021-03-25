// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Properties;
using Axe.Windows.Core.Bases;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace AccessibilityInsights.SharedUx.Converters
{
    /// <summary>
    /// Convert Element to Sender Text
    /// </summary>
    [ValueConversion(typeof(A11yElement), typeof(string))]
    public class ElementToSenderTextConverter : MarkupExtension, IValueConverter
    {
        private static ElementToSenderTextConverter _instance;

        public ElementToSenderTextConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        { // do not let the culture default to local to prevent variable outcome re decimal syntax
            var e = (A11yElement) value;
            return e != null ? e.Glimpse : Resources.ElementToSenderTextConverter_Convert_Event_Recorder;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { // read only converter...
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new ElementToSenderTextConverter());
        }
    }
}
