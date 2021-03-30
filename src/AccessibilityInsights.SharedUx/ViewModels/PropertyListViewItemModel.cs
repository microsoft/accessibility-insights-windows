// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using System.Globalization;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// PropertyListViewItem class
    /// ViewModel for ElementProperty in Property ListView
    /// </summary>
    public class PropertyListViewItemModel : ViewModelBase
    {
        public A11yProperty Property { get; private set; }

        public string Name
        {
            get
            {
                return this.Property.Name;
            }
        }

        public string Value
        {
            get
            {
                return this.Property.TextValue;
            }
        }

        public string AutomationName
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}: {1}", Name, Value ?? Properties.Resources.PropertyDoesNotExist);
            }
        }

        public PropertyListViewItemModel(A11yProperty p)
        {
            this.Property = p;
        }

        /// <summary>
        /// Clear
        /// </summary>
        public void Clear()
        {
            this.Property = null;
        }

        /// <summary>
        /// Override the class name so it'd be usable accessible in the list view names
        /// </summary>
        public override string ToString()
        {
            return this.AutomationName;
        }
    }
}
