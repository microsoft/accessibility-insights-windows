// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using UIAutomationClient;
using Axe.Windows.Core.Attributes;
using Axe.Windows.Desktop.Utility;
using Axe.Windows.Telemetry;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Axe.Windows.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Wrapper for IUIAutomationTextRange
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671377(v=vs.85).aspx
    /// </summary>
    public class TextRange : IDisposable
    {
        /// <summary>
        /// UIAInterface of TextRange
        /// </summary>
        public IUIAutomationTextRange UIATextRange { get; private set; }

        /// <summary>
        /// TextPattern which this Range was from
        /// </summary>
        public TextPattern TextPattern { get; private set; }

        public TextRange(IUIAutomationTextRange tr, TextPattern tp)
        {
            UIATextRange = tr;
            this.TextPattern = tp;
        }

        [PatternMethod]
        public void Select()
        {
            this.UIATextRange.Select();
        }

        [PatternMethod]
        public void AddToSelection()
        {
            this.UIATextRange.AddToSelection();
        }

        [PatternMethod]
        public void RemoveFromSelection()
        {
            this.UIATextRange.RemoveFromSelection();
        }

        [PatternMethod]
        public void ScrollIntoView(bool alignToTop)
        {
            this.UIATextRange.ScrollIntoView(alignToTop ? 1 : 0);
        }

        [PatternMethod]
        public List<DesktopElement> GetChildren()
        {
            return this.UIATextRange.GetChildren()?.ToListOfDesktopElements();
        }

        [PatternMethod]
        public DesktopElement GetEnclosingElement()
        {
            return new DesktopElement(this.UIATextRange.GetEnclosingElement());
        }

        [PatternMethod]
        public int Move(TextUnit unit, int count)
        {
            return this.UIATextRange.Move(unit, count);
        }

        [PatternMethod]
        public void MoveEndpointByRange(TextPatternRangeEndpoint srcEndPoint, TextRange tr, TextPatternRangeEndpoint targetEndPoint)
        {
            this.UIATextRange.MoveEndpointByRange(srcEndPoint, tr.UIATextRange, targetEndPoint);
        }

        [PatternMethod]
        public int MoveEndpointByUnit(TextPatternRangeEndpoint endpoint, TextUnit unit, int count)
        {
            return this.UIATextRange.MoveEndpointByUnit(endpoint, unit, count);
        }

        [PatternMethod]
        public void ExpandToEnclosingUnit(TextUnit tu)
        {
            this.UIATextRange.ExpandToEnclosingUnit(tu);
        }

        [PatternMethod]
        public TextRange Clone()
        {
            return new TextRange(this.UIATextRange.Clone(), this.TextPattern);
        }

        [PatternMethod]
        public bool Compare(TextRange tr)
        {
            return Convert.ToBoolean(this.UIATextRange.Compare(tr.UIATextRange));
        }

        [PatternMethod]
        public int CompareEndpoints(TextPatternRangeEndpoint srcEndPoint, TextRange tr, TextPatternRangeEndpoint targetEndPoint)
        {
            return this.UIATextRange.CompareEndpoints(srcEndPoint, tr.UIATextRange, targetEndPoint);
        }

        [PatternMethod]
        public TextRange FindAttribute(int attr, object val, bool backward)
        {
            var uiatr = this.UIATextRange.FindAttribute(attr, val, backward ? 1 : 0);
            return uiatr != null ? new TextRange(uiatr, this.TextPattern) : null;
        }

        [PatternMethod]
        public TextRange FindText(string text, bool backward, bool ignoreCase)
        {
            var uiatr = this.UIATextRange.FindText(text, backward ? 1 : 0, ignoreCase ? 1 : 0);
            return uiatr != null ? new TextRange(uiatr, this.TextPattern) : null;
        }

        /// <summary>
        /// it is not still clear on how we will handle GetAttributeValue(). 
        /// ToDo Item.
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        [PatternMethod]
        public dynamic GetAttributeValue(int attr)
        {
            try
            {
                return this.UIATextRange.GetAttributeValue(attr);
            }
            catch(Exception e)
            {
                e.ReportException();
            }

            return null;
        }

        [PatternMethod]
        public List<Rectangle> GetBoundingRectangles()
        {
            List<Rectangle> list = new List<Rectangle>();

            var arr = this.UIATextRange.GetBoundingRectangles();
            for (int i = 0; i < arr.Length; i += 4)
            {
                list.Add(new Rectangle(Convert.ToInt32((double)arr.GetValue(i)), Convert.ToInt32((double)arr.GetValue(i + 1)), Convert.ToInt32((double)arr.GetValue(i + 2)), Convert.ToInt32((double)arr.GetValue(i + 3))));
            }

            return list;
        }

        [PatternMethod]
        public string GetText(int max)
        {
            return this.UIATextRange.GetText(max);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if(this.UIATextRange != null)
                {
                    Marshal.ReleaseComObject(UIATextRange);
                    UIATextRange = null;
                }
                disposedValue = true;
            }
        }

        ~TextRange()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
