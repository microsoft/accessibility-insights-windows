// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Core.Bases;
using UIAutomationClient;
using AccessibilityInsights.Core.Attributes;

namespace AccessibilityInsights.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for ObjectModel Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/hh448771(v=vs.85).aspx
    /// though it has a method to call, it may not be actionable in AT perspective. 
    /// </summary>
    public class ObjectModelPattern : A11yPattern
    {
        IUIAutomationObjectModelPattern Pattern = null;

        public ObjectModelPattern(A11yElement e, IUIAutomationObjectModelPattern p) : base(e, PatternType.UIA_ObjectModelPatternId)
        {
            Pattern = p;
        }

        [PatternMethod]
        public dynamic GetUnderlyingObjectModel()
        {
            return this.Pattern.GetUnderlyingObjectModel();
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
