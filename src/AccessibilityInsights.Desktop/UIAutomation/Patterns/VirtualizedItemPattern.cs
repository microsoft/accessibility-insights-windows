// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Core.Bases;
using UIAutomationClient;
using AccessibilityInsights.Core.Attributes;

namespace AccessibilityInsights.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for VirtualizedItem Control Pattern
    /// </summary>
    public class VirtualizedItemPattern : A11yPattern
    {
        IUIAutomationVirtualizedItemPattern Pattern = null;

        public VirtualizedItemPattern(A11yElement e, IUIAutomationVirtualizedItemPattern p) : base(e, PatternType.UIA_VirtualizedItemPatternId)
        {
            Pattern = p;
        }

        [PatternMethod]
        public void Realize()
        {
            this.Pattern.Realize();
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
