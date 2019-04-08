// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using Axe.Windows.Core.Bases;
using UIAutomationClient;
using Axe.Windows.Core.Attributes;

namespace Axe.Windows.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for Invoke Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671279(v=vs.85).aspx
    /// </summary>
    public class InvokePattern : A11yPattern
    {
        IUIAutomationInvokePattern Pattern = null;

        public InvokePattern(A11yElement e, IUIAutomationInvokePattern p) : base(e, PatternType.UIA_InvokePatternId)
        {
            Pattern = p;
            this.IsUIActionable = true;
        }

        [PatternMethod(IsUIAction = true)]
        public void Invoke()
        {
            this.Pattern.Invoke();
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
