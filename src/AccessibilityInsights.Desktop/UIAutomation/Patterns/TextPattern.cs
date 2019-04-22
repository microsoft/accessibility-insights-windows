// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using System.Collections.Generic;
using Axe.Windows.Core.Bases;
using UIAutomationClient;
using Axe.Windows.Core.Attributes;
using Axe.Windows.Desktop.Types;

namespace Axe.Windows.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for Text Control Pattern
    /// </summary>
    [PatternEvent(Id = EventType.UIA_Text_TextChangedEventId)]
    [PatternEvent(Id = EventType.UIA_Text_TextSelectionChangedEventId)]
    public class TextPattern : A11yPattern
    {
        IUIAutomationTextPattern Pattern = null;

        public TextPattern(A11yElement e, IUIAutomationTextPattern p) : base(e, PatternType.UIA_TextPatternId)
        {
            Pattern = p;

            PopulateProperties();

            // if textPattern is supported , it means that user would do select text in the control. 
            // so it should be marked as UI actionable
            IsUIActionable = true;
        }

        private void PopulateProperties()
        {
            ExcludeExceptionsFromTelemetry(() =>
            {
                this.Properties.Add(new A11yPatternProperty() { Name = "SupportedTextSelection", Value = this.Pattern.SupportedTextSelection });
            });
        }

        [PatternMethod]
        public TextRange DocumentRange()
        {
            return new TextRange(this.Pattern.DocumentRange, this);
        }

        [PatternMethod]
        public List<TextRange> GetSelection()
        {
            return ToListOfTextRanges(this.Pattern.GetSelection());
        }

        [PatternMethod]
        public List<TextRange> GetVisibleRanges()
        {
            return ToListOfTextRanges(this.Pattern.GetVisibleRanges());
        }

        [PatternMethod]
        public TextRange RangeFromChild(DesktopElement child)
        {
            return new TextRange(this.Pattern.RangeFromChild(child.PlatformObject),this);
        }

        [PatternMethod]
        public TextRange RangeFromPoint(tagPOINT pt)
        {
            return new TextRange(this.Pattern.RangeFromPoint(pt),this);
        }

        List<TextRange> ToListOfTextRanges(IUIAutomationTextRangeArray array)
        {
            List<TextRange> list = new List<TextRange>();

            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    list.Add(new TextRange(array.GetElement(i), this));
                }
            }

            return list;
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
