// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Desktop.Types;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace AccessibilityInsights.SharedUx.Converters
{
    /// <summary>
    /// Convert EventMessage to EventName
    /// </summary>
    [ValueConversion(typeof(int), typeof(string))]
    public class EventIdToEventNameConverter : MarkupExtension, IValueConverter
    {
        private static EventIdToEventNameConverter _instance;

        public EventIdToEventNameConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        { // do not let the culture default to local to prevent variable outcome re decimal syntax
            var eId = (int)value;
            return EventType.GetInstance().GetNameById(eId);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { // read only converter...
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new EventIdToEventNameConverter());
        }
    }
}
