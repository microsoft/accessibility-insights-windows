// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace AccessibilityInsights.SharedUx.Converters
{
    /// <summary>
    /// Converter used to ensure a column isn't wider than the available space
    /// </summary>
    [ValueConversion(typeof(double), typeof(double))]
    public class ColumnMaxWidthSpacingConverter : MarkupExtension, IValueConverter
    {
        private static ColumnMaxWidthSpacingConverter _instance;

        internal const int SpacingConstant = 4;

        public ColumnMaxWidthSpacingConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var candidateResult = (double)value - SpacingConstant;
            return candidateResult >= 0 ? candidateResult : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (double)value + SpacingConstant;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new ColumnMaxWidthSpacingConverter());
        }
    }
}
