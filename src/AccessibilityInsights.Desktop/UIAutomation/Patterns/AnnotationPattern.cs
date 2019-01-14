// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Attributes;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Types;
using System.Runtime.InteropServices;
using UIAutomationClient;

namespace AccessibilityInsights.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for Annotation Control Pattern
    /// No actiona is available
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/hh448769(v=vs.85).aspx
    /// </summary>
    public class AnnotationPattern:A11yPattern
    {
        IUIAutomationAnnotationPattern Pattern = null;

        public AnnotationPattern(A11yElement e, IUIAutomationAnnotationPattern p) :base(e, PatternType.UIA_AnnotationPatternId)
        {
            Pattern = p;

            PopulateProperties();
        }

        private void PopulateProperties()
        {
            this.Properties.Add(new A11yPatternProperty() { Name = "AnnotationTypeId", Value = this.Pattern.CurrentAnnotationTypeId });
            this.Properties.Add(new A11yPatternProperty() { Name = "AnnotationTypeName", Value = this.Pattern.CurrentAnnotationTypeName });
            this.Properties.Add(new A11yPatternProperty() { Name = "Author", Value = this.Pattern.CurrentAuthor });
            this.Properties.Add(new A11yPatternProperty() { Name = "DateTime", Value = this.Pattern.CurrentDateTime });
        }

        [PatternMethod]
        public DesktopElement GetTarget()
        {
            return new DesktopElement(this.Pattern.CurrentTarget);
        }

        protected override void Dispose(bool disposing)
        {
            if(Pattern != null)
            {
                Marshal.ReleaseComObject(Pattern);
                this.Pattern = null;
            }

            base.Dispose(disposing);
        }
    }
}
