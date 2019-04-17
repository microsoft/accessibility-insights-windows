// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using System.Collections.Generic;
using Axe.Windows.Core.Bases;
using UIAutomationClient;
using Axe.Windows.Core.Attributes;
using Axe.Windows.Desktop.Misc;

namespace Axe.Windows.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for TableItem Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671289(v=vs.85).aspx
    /// </summary>
    public class TableItemPattern : A11yPattern
    {
        IUIAutomationTableItemPattern Pattern = null;

        public TableItemPattern(A11yElement e, IUIAutomationTableItemPattern p) : base(e, PatternType.UIA_TableItemPatternId)
        {
            Pattern = p;
        }

        [PatternMethod]
        public List<DesktopElement> GetColumnHeaderItems()
        {
            return this.Pattern.GetCurrentColumnHeaderItems()?.ToListOfDesktopElements();
        }

        [PatternMethod]
        public List<DesktopElement> GetRowHeaderItems()
        {
            return this.Pattern.GetCurrentRowHeaderItems()?.ToListOfDesktopElements();
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
