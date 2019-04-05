// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using Axe.Windows.Core.Bases;
using UIAutomationClient;
using Axe.Windows.Core.Attributes;

namespace Axe.Windows.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for Toggle Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671290(v=vs.85).aspx
    /// </summary>
    public class TogglePattern : A11yPattern
    {
        IUIAutomationTogglePattern Pattern = null;

        public TogglePattern(A11yElement e, IUIAutomationTogglePattern p) : base(e, PatternType.UIA_TogglePatternId)
        {
            Pattern = p;
            PopulateProperties();
        }

        private void PopulateProperties()
        {
            this.Properties.Add(new A11yPatternProperty() { Name = "ToggleState", Value = this.Pattern.CurrentToggleState });
        }

        [PatternMethod(IsUIAction = true)]
        public void Toggle()
        {
            this.Pattern.Toggle();
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
