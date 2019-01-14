// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using System;
using System.Collections.Generic;
using AccessibilityInsights.Core.Bases;
using UIAutomationClient;
using AccessibilityInsights.Core.Attributes;
using AccessibilityInsights.Desktop.Utility;
using AccessibilityInsights.Desktop.Types;

namespace AccessibilityInsights.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for Selection Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671285(v=vs.85).aspx
    /// </summary>
    [PatternEvent(Id = EventType.UIA_Selection_InvalidatedEventId)]
    public class SelectionPattern : A11yPattern
    {
        IUIAutomationSelectionPattern Pattern = null;

        public SelectionPattern(A11yElement e, IUIAutomationSelectionPattern p) : base(e, PatternType.UIA_SelectionPatternId)
        {
            Pattern = p;

            PopulateProperties();
        }

        private void PopulateProperties()
        {
            this.Properties.Add(new A11yPatternProperty() { Name = "CanSelectMultiple", Value = Convert.ToBoolean(this.Pattern.CurrentCanSelectMultiple) });
            this.Properties.Add(new A11yPatternProperty() { Name = "IsSelectionRequired", Value = Convert.ToBoolean(this.Pattern.CurrentIsSelectionRequired) });
        }

        [PatternMethod]
        public List<DesktopElement> GetSelection()
        {
            return this.Pattern.GetCurrentSelection().ToListOfDesktopElements();
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
