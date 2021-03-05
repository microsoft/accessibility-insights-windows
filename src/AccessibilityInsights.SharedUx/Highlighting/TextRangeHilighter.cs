// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AccessibilityInsights.SharedUx.Highlighting
{
    /// <summary>
    /// class TextRangeHilighter
    /// Hiligt all bounding rectangles from a Text Range
    /// </summary>
    public class TextRangeHilighter: IDisposable
    {
        /// <summary>
        /// Hilighter color
        /// </summary>
        private HighlighterColor Color;

        private List<Highlighter> Hilighters;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="color"></param>
        public TextRangeHilighter(HighlighterColor color = HighlighterColor.TextBrush)
        {
            this.Color = color;
            this.Hilighters = new List<Highlighter>();
        }

        /// <summary>
        /// Set the boundingrectangles to hilight
        /// </summary>
        public void SetBoundingRectangles(IList<Rectangle> rects)
        {
            if (Hilighters.Count != 0)
            {
                this.Hilighters.ForEach(hl => hl.Dispose());
                this.Hilighters.Clear();
            }

            var list = from br in rects
                       where br.IsVisibleLocation()
                       select br;

            foreach (var l in list)
            {
                var hl = new Highlighter(this.Color) { IsVisible = false };
                hl.SetLocation(l);
                this.Hilighters.Add(hl);
            }
        }

        /// <summary>
        /// Hilight BoundingRectangles
        /// </summary>
        /// <param name="isVisible">hlight when it is true</param>
        public void HilightBoundingRectangles(bool isVisible)
        {
            this.Hilighters?.ForEach(hl => hl.IsVisible = isVisible);
        }

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this.Hilighters != null)
                    {
                        this.Hilighters.ForEach(h => h.Dispose());
                        this.Hilighters.Clear();
                    }
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
