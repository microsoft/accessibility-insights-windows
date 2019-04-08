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
    /// Control pattern wrapper for TextChild Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/dn313199(v=vs.85).aspx
    /// </summary>
    [PatternEvent(Id = EventType.UIA_TextEdit_TextChangedEventId)]
    [PatternEvent(Id = EventType.UIA_TextEdit_ConversionTargetChangedEventId)]
    public class TextEditPattern : A11yPattern
    {
        IUIAutomationTextEditPattern Pattern = null;

        public TextEditPattern(A11yElement e, IUIAutomationTextEditPattern p) : base(e, PatternType.UIA_TextEditPatternId)
        {
            Pattern = p;
        }

        [PatternMethod]
        public TextRange GetActiveComposition()
        {
            var tr = this.Pattern.GetActiveComposition();
            return tr != null ? new TextRange(tr, null) : null ;            
        }

        [PatternMethod]
        public TextRange GetConversionTarget()
        {
            var tr = this.Pattern.GetConversionTarget();
            return tr != null ? new TextRange(tr, null) : null;
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
