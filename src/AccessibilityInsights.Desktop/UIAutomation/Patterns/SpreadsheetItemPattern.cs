// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using System.Collections.Generic;
using AccessibilityInsights.Core.Bases;
using UIAutomationClient;
using AccessibilityInsights.Core.Attributes;
using AccessibilityInsights.Desktop.Utility;
using AccessibilityInsights.Desktop.Types;

namespace AccessibilityInsights.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for SpreadsheetItem Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/hh448774(v=vs.85).aspx
    /// </summary>
    public class SpreadsheetItemPattern : A11yPattern
    {
        IUIAutomationSpreadsheetItemPattern Pattern = null;

        public SpreadsheetItemPattern(A11yElement e, IUIAutomationSpreadsheetItemPattern p) : base(e, PatternType.UIA_SpreadsheetItemPatternId)
        {
            Pattern = p;

            PopulateProperties();
        }

        private void PopulateProperties()
        {
            this.Properties.Add(new A11yPatternProperty() { Name = "Formula", Value = this.Pattern.CurrentFormula });
        }

        [PatternMethod]
        public List<DesktopElement> GetAnnotationObjects()
        {
            return this.Pattern.GetCurrentAnnotationObjects()?.ToListOfDesktopElements();
        }

        [PatternMethod]
        public List<string> GetAnnotationTypes()
        {
            var array = this.Pattern.GetCurrentAnnotationTypes();
            List<string> list = new List<string>();

            if(array.Length > 0)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    list.Add(AnnotationType.GetInstance().GetNameById((int)array.GetValue(i)));
                }
            }

            return list;
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
