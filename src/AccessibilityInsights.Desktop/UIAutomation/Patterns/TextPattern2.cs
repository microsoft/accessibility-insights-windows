// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using Axe.Windows.Core.Bases;
using UIAutomationClient;
using Axe.Windows.Core.Attributes;

namespace Axe.Windows.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for Text Control Pattern
    /// </summary>
    public class TextPattern2 : A11yPattern
    {
        IUIAutomationTextPattern2 Pattern = null;

        public TextPattern2(A11yElement e, IUIAutomationTextPattern2 p) : base(e, PatternType.UIA_TextPattern2Id)
        {
            Pattern = p;
        }

        [PatternMethod]
        public TextRange GetCaretRange(out int isActive)
        {
            return new TextRange(this.Pattern.GetCaretRange(out isActive), null);
        }

        [PatternMethod]
        public TextRange RangeFromAnnotation(A11yElement e)
        {
            return new TextRange(this.Pattern.RangeFromAnnotation(e.PlatformObject), null);
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
