// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Attributes;
using Axe.Windows.Desktop.Types;
using Axe.Windows.Telemetry;
using System.Runtime.InteropServices;
using UIAutomationClient;

namespace Axe.Windows.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for ExpandCollapse Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671276(v=vs.85).aspx
    /// </summary>
    [PatternEvent(Id = EventType.UIA_AutomationPropertyChangedEventId)]
    public class ExpandCollapsePattern : A11yPattern
    {
        IUIAutomationExpandCollapsePattern Pattern = null;

        public ExpandCollapsePattern(A11yElement e, IUIAutomationExpandCollapsePattern p) : base(e, PatternType.UIA_ExpandCollapsePatternId)
        {
            Pattern = p;

            PopulateProperties();
        }

        private void PopulateProperties()
        {
            ExceptionExcluder.ExcludeThrownExceptions(() =>
            {
                this.Properties.Add(new A11yPatternProperty() { Name = "ExpandCollapseState", Value = this.Pattern.CurrentExpandCollapseState });
            });
        }

        [PatternMethod(IsUIAction = true)]
        public void Expand()
        {
            this.Pattern.Expand();
        }

        [PatternMethod(IsUIAction = true)]
        public void Collapse()
        {
            this.Pattern.Collapse();
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
