// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using System;
using AccessibilityInsights.Core.Bases;
using UIAutomationClient;
using AccessibilityInsights.Core.Attributes;
using AccessibilityInsights.Desktop.Types;

namespace AccessibilityInsights.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for Window Control Pattern
    /// </summary>
    [PatternEvent(Id = EventType.UIA_Window_WindowClosedEventId)]
    [PatternEvent(Id = EventType.UIA_Window_WindowOpenedEventId)]
    public class WindowPattern : A11yPattern
    {
        IUIAutomationWindowPattern Pattern = null;

        public WindowPattern(A11yElement e, IUIAutomationWindowPattern p) : base(e, PatternType.UIA_WindowPatternId)
        {
            Pattern = p;

            PopulateProperties();
        }

        private void PopulateProperties()
        {
            this.Properties.Add(new A11yPatternProperty() { Name = "CanMaximize", Value = Convert.ToBoolean(this.Pattern.CurrentCanMaximize) });
            this.Properties.Add(new A11yPatternProperty() { Name = "CanMinimize", Value = Convert.ToBoolean(this.Pattern.CurrentCanMinimize) });
            this.Properties.Add(new A11yPatternProperty() { Name = "IsModal", Value = Convert.ToBoolean(this.Pattern.CurrentIsModal) });
            this.Properties.Add(new A11yPatternProperty() { Name = "IsTopmost", Value = Convert.ToBoolean(this.Pattern.CurrentIsTopmost) });
            this.Properties.Add(new A11yPatternProperty() { Name = "WindowInteractionState", Value = this.Pattern.CurrentWindowInteractionState });
            this.Properties.Add(new A11yPatternProperty() { Name = "WindowVisualState", Value = this.Pattern.CurrentWindowVisualState });
        }

        [PatternMethod]
        public void SetWindowVisualState(WindowVisualState state)
        {
            this.Pattern.SetWindowVisualState(state);
        }

        /// <summary>
        /// Wait for Input Idel
        /// </summary>
        /// <param name="ms">miliseconds</param>
        [PatternMethod]
        public void WaitForInputIdle(int ms)
        {
            this.Pattern.WaitForInputIdle(ms);
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
