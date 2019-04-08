// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using System.Collections.Generic;
using UIAutomationClient;

namespace Axe.Windows.Desktop.UIAutomation
{
    /// <summary>
    /// Class DesktopElementHelper
    /// it is a helper class for DesktopElement to handle property/pattern related caching and more
    /// it is a singleton object via static members
    /// </summary>
    public static class DesktopElementHelper
    {

        /// <summary>
        /// List of basic properties 
        /// these will be cached at instantiation of Element
        /// </summary>
        static List<int> sDefaultCoreProperties = new List<int>()
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

        static IEnumerable<int> sCoreProperties = null;

        /// <summary>
        /// Set the selected properties list
        /// </summary>
        /// <param name="pps"></param>
        public static void SetCorePropertiesList(IEnumerable<int> pps)
        {
            sCoreProperties = pps;
        }

        /// <summary>
        /// Get the selected properties list
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<int> GetCorePropertiesList()
        {
            return sCoreProperties;
        }

        /// <summary>
        /// Build a cacherequest for properties and patterns
        /// </summary>
        /// <param name="pps">Property ids</param>
        /// <param name="pts">Pattern ids</param>
        public static IUIAutomationCacheRequest BuildCacheRequest(List<int> pps, List<int> pts)
        {
            return GetPropertiesCache(A11yAutomation.GetUIAutomationObject(), pps, pts);
        }

        /// <summary>
        /// Build a cacherequest for properties and patterns
        /// </summary>
        /// <param name="uia"></param>
        /// <param name="pps">Property ids</param>
        /// <param name="pts">Pattern ids</param>
        /// <returns></returns>
        public static IUIAutomationCacheRequest GetPropertiesCache(CUIAutomation uia, List<int> pps, List<int> pts)
        {
            var cr = uia.CreateCacheRequest();

            if (pps != null)
            {
                foreach (var pp in pps)
                {
                    cr.AddProperty(pp);
                }
            }

            if (pts != null)
            {
                foreach (var pt in pts)
                {
                    if (pt != 0)
                    {
                        cr.AddPattern(pt);
                    }
                }
            }

            return cr;
        }

        /// <summary>
        /// Get the default list of selected properties
        /// </summary>
        /// <returns></returns>
        public static List<int> GetDefaultCoreProperties()
        {
            return sDefaultCoreProperties;
        }

    }
}
