// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace AccessibilityInsights.SharedUx.Converters
{
    /// <summary>
    /// Creates proper margin for TreeViewItem spacing based on node depth
    /// </summary>
    [ValueConversion(typeof(HierarchyNodeViewModel), typeof(Thickness))]
    public class TreeNodeToMarginConverter : MarkupExtension, IValueConverter
    {
        private static TreeNodeToMarginConverter _instance;

        public TreeNodeToMarginConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var nodeDepth = 0;
            if (value != null && value is HierarchyNodeViewModel vm)
            {
                var el = vm.Element;

                while (el.Parent != null)
                {
                    nodeDepth++;
                    el = el.Parent;
                }
            }
            else if (value is PatternPropertyUIWrapper)
            {
                // intent pattern properties
                nodeDepth = 1;
            }
            else if (value is EventConfigNodeViewModel evnvm)
            {
                // intent event recording config
                nodeDepth = evnvm.Depth;
            }
            return new Thickness(nodeDepth * -16, 0,0,0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { // read only converter...
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new TreeNodeToMarginConverter());
        }
    }
}
