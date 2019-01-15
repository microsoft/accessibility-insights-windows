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
    /// DockPattern 
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671421(v=vs.85).aspx
    /// </summary>
    public class DockPattern : A11yPattern
    {
        IUIAutomationDockPattern Pattern = null;

        public DockPattern(A11yElement e, IUIAutomationDockPattern p) : base(e, PatternType.UIA_DockPatternId)
        {
            this.Pattern = p;
            PopulateProperties();
        }

        private void PopulateProperties()
        {
            this.Properties.Add(new A11yPatternProperty() { Name = "DockPosition", Value = this.Pattern.CurrentDockPosition });
        }

        [PatternMethod(IsUIAction = true)]
        public void SetDockPosition(DockPosition dockPos)
        {
            this.Pattern.SetDockPosition(dockPos);
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
