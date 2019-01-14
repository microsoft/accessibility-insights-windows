// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Core.Bases;
using UIAutomationClient;
using AccessibilityInsights.Core.Attributes;

namespace AccessibilityInsights.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for Scroll Item Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671284(v=vs.85).aspx
    /// </summary>
    public class ScrollItemPattern : A11yPattern
    {
        IUIAutomationScrollItemPattern Pattern = null;

        public ScrollItemPattern(A11yElement e, IUIAutomationScrollItemPattern p) : base(e, PatternType.UIA_ScrollItemPatternId)
        {
            Pattern = p;
        }

        /// <summary>
        /// this method is not UI Action by user. but it is for UI automation.
        /// </summary>
        [PatternMethod]
        public void ScrollIntoView()
        {
            this.Pattern.ScrollIntoView();
        }

        protected override void Dispose(bool disposing)
        {
            if (Pattern != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(Pattern);
                this.Pattern = null;
            }

            base.Dispose(disposing);
        }
    }
}
