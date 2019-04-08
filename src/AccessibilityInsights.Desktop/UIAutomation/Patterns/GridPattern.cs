// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using Axe.Windows.Core.Bases;
using UIAutomationClient;
using Axe.Windows.Core.Attributes;

namespace Axe.Windows.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for Grid Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671277(v=vs.85).aspx
    /// </summary>
    public class GridPattern : A11yPattern
    {
        IUIAutomationGridPattern Pattern = null;

        public GridPattern(A11yElement e, IUIAutomationGridPattern p) : base(e, PatternType.UIA_GridPatternId)
        {
            Pattern = p;

            PopulateProperties();
        }

        private void PopulateProperties()
        {
            this.Properties.Add(new A11yPatternProperty() { Name = "ColumnCount", Value = this.Pattern.CurrentColumnCount });
            this.Properties.Add(new A11yPatternProperty() { Name = "RowCount", Value = this.Pattern.CurrentRowCount });
        }

        [PatternMethod]
        public DesktopElement GetItem(int row, int column)
        {
            var uiae = this.Pattern.GetItem(row, column);

            return uiae != null ? new DesktopElement(uiae) : null;
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
