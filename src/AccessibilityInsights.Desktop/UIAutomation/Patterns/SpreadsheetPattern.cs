// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using Axe.Windows.Core.Bases;
using UIAutomationClient;
using Axe.Windows.Core.Attributes;

namespace Axe.Windows.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for Spreadsheet Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/hh448773(v=vs.85).aspx
    /// </summary>
    public class SpreadsheetPattern : A11yPattern
    {
        IUIAutomationSpreadsheetPattern Pattern = null;

        public SpreadsheetPattern(A11yElement e, IUIAutomationSpreadsheetPattern p) : base(e, PatternType.UIA_SpreadsheetPatternId)
        {
            Pattern = p;
        }

        [PatternMethod]
        public DesktopElement GetItemByName(string name)
        {
            return new DesktopElement(this.Pattern.GetItemByName(name));
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
