// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.ActionViews
{
    public class DynamicDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate
            SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is Parameter)
            {
                var model = item as Parameter;

                return (DataTemplate)element.FindResource(model.ParamType.IsEnum ? "ComboBoxTemplate" : "TextBoxTemplate");
            }

            return null;
        }
    }
}
