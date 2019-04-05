// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using Axe.Windows.Core.Bases;
using UIAutomationClient;
using Axe.Windows.Core.Attributes;

namespace Axe.Windows.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for TextChild Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/hh448740(v=vs.85).aspx
    /// </summary>
    public class TextChildPattern : A11yPattern
    {
        IUIAutomationTextChildPattern Pattern = null;

        public TextChildPattern(A11yElement e, IUIAutomationTextChildPattern p) : base(e, PatternType.UIA_TextChildPatternId)
        {
            Pattern = p;
        }

        [PatternMethod]
        public DesktopElement TextContainer()
        {
            return new DesktopElement(this.Pattern.TextContainer, true, true);
        }

        [PatternMethod]
        public TextRange TextRange()
        {
            return new TextRange(this.Pattern.TextRange, null);
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
