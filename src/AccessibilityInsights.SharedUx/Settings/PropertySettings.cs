// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Axe.Windows.Core.Types;
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.SharedUx.Settings
{
    public class PropertySettings
    {
        /// <summary>
        /// Default properties to display if no config exists
        /// </summary>
        internal static readonly IEnumerable<int> DefaultCoreProperties = new List<int>
        {
            PropertyType.UIA_NamePropertyId,
            PropertyType.UIA_ControlTypePropertyId,
            PropertyType.UIA_LocalizedControlTypePropertyId,
            PropertyType.UIA_IsKeyboardFocusablePropertyId,
            PropertyType.UIA_BoundingRectanglePropertyId,
            PropertyType.UIA_AccessKeyPropertyId,
            PropertyType.UIA_AcceleratorKeyPropertyId,
            PropertyType.UIA_HelpTextPropertyId,
            PropertyType.UIA_AriaPropertiesPropertyId,
            PropertyType.UIA_AriaRolePropertyId,
        };

        /// <summary>
        /// Currently configured properties to display
        /// </summary>
        public IEnumerable<int> SelectedCoreProperties { get; set; }

        public PropertySettings()
        {
            SelectedCoreProperties = Array.Empty<int>();
        }
    }
}
