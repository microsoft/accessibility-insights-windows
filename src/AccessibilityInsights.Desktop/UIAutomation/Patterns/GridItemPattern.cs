// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using System;
using Axe.Windows.Core.Bases;
using UIAutomationClient;
using Axe.Windows.Core.Attributes;

namespace Axe.Windows.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for GridItem Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671278(v=vs.85).aspx
    /// </summary>
    public class GridItemPattern : A11yPattern
    {
        IUIAutomationGridItemPattern Pattern = null;

        public GridItemPattern(A11yElement e, IUIAutomationGridItemPattern p) : base(e, PatternType.UIA_GridItemPatternId)
        {
            Pattern = p;

            PopulateProperties();
        }

        private void PopulateProperties()
        {
            this.Properties.Add(new A11yPatternProperty() { Name = "Column", Value = this.Pattern.CurrentColumn });
            this.Properties.Add(new A11yPatternProperty() { Name = "ColumnSpan", Value = this.Pattern.CurrentColumnSpan });
            this.Properties.Add(new A11yPatternProperty() { Name = "Row", Value = this.Pattern.CurrentRow });
            this.Properties.Add(new A11yPatternProperty() { Name = "RowSpan", Value = this.Pattern.CurrentRowSpan });
        }

        [PatternMethod]
        public DesktopElement GetContainingGrid()
        {
            var e = new DesktopElement(this.Pattern.CurrentContainingGrid);

            if (e.Properties != null && e.Properties.Count != 0)
            {
                return e;
            }

            throw new ApplicationException("Pattern may not be valid any more.");
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
