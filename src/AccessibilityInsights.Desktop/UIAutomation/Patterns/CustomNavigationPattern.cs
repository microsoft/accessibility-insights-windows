// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Core.Bases;
using UIAutomationClient;
using AccessibilityInsights.Core.Attributes;
using System.Runtime.InteropServices;

namespace AccessibilityInsights.Desktop.UIAutomation.Patterns
{

    /// <summary>
    /// CustomNavigation Pattern 
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/dn858171(v=vs.85).aspx
    /// </summary>
    class CustomNavigationPattern :A11yPattern
    {
        IUIAutomationCustomNavigationPattern Pattern = null;

        public CustomNavigationPattern(A11yElement e, IUIAutomationCustomNavigationPattern p) :base(e, PatternType.UIA_CustomNavigationPatternId)
        {
            this.Pattern = p;
        }

        /// <summary>
        /// Navigate action
        /// </summary>
        /// <param name="direction"></param>
        [PatternMethod(IsUIAction = true)]
        public void Navigate(NavigateDirection direction)
        {
            this.Pattern.Navigate(direction);
        }

        protected override void Dispose(bool disposing)
        {
            if (Pattern != null)
            {
                Marshal.ReleaseComObject(Pattern);
                this.Pattern = null;
            }

            base.Dispose(disposing);
        }
    }
}
