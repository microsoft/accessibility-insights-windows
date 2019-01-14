// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using System.Collections.Generic;
using AccessibilityInsights.Core.Bases;
using UIAutomationClient;
using AccessibilityInsights.Desktop.Utility;
using AccessibilityInsights.Core.Attributes;

namespace AccessibilityInsights.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for Table Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671288(v=vs.85).aspx
    /// </summary>
    public class TablePattern : A11yPattern
    {
        IUIAutomationTablePattern Pattern = null;

        public TablePattern(A11yElement e, IUIAutomationTablePattern p) : base(e, PatternType.UIA_TablePatternId)
        {
            Pattern = p;

            PopulateProperties();
        }

        private void PopulateProperties()
        {
            this.Properties.Add(new A11yPatternProperty() { Name = "RowOrColumnMajor", Value = this.Pattern.CurrentRowOrColumnMajor });
        }

        [PatternMethod]
        public List<DesktopElement> GetColumnHeaders()
        {
            return this.Pattern.GetCurrentColumnHeaders()?.ToListOfDesktopElements();
        }

        [PatternMethod]
        public List<DesktopElement> GetRowHeaders()
        {
            return this.Pattern.GetCurrentRowHeaders()?.ToListOfDesktopElements();
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
