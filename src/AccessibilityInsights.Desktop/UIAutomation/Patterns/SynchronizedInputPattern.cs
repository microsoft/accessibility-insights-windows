// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using Axe.Windows.Core.Bases;
using UIAutomationClient;
using Axe.Windows.Core.Attributes;
using Axe.Windows.Desktop.Types;

namespace Axe.Windows.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for SynchronizedInput Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671287(v=vs.85).aspx
    /// </summary>
    [PatternEvent(Id = EventType.UIA_InputReachedTargetEventId)]
    public class SynchronizedInputPattern : A11yPattern
    {
        IUIAutomationSynchronizedInputPattern Pattern = null;

        public SynchronizedInputPattern(A11yElement e, IUIAutomationSynchronizedInputPattern p) : base(e, PatternType.UIA_SynchronizedInputPatternId)
        {
            Pattern = p;
        }

        [PatternMethod]
        public void StartListening(SynchronizedInputType inputType)
        {
            this.Pattern.StartListening(inputType);
        }

        [PatternMethod]
        public void Cancel()
        {
            this.Pattern.Cancel();
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
