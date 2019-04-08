// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using Axe.Windows.Core.Bases;
using UIAutomationClient;

namespace Axe.Windows.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for Styles Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/hh448775(v=vs.85).aspx
    /// </summary>
    public class StylesPattern : A11yPattern
    {
        IUIAutomationStylesPattern Pattern = null;

        public StylesPattern(A11yElement e, IUIAutomationStylesPattern p) : base(e, PatternType.UIA_StylesPatternId)
        {
            Pattern = p;

            PopulateProperties();
        }

        private void PopulateProperties()
        {
            this.Properties.Add(new A11yPatternProperty() { Name = "ExtendedProperties", Value = this.Pattern.CurrentExtendedProperties });
            this.Properties.Add(new A11yPatternProperty() { Name = "FillColor ", Value = this.Pattern.CurrentFillColor });
            this.Properties.Add(new A11yPatternProperty() { Name = "FillPatternColor ", Value = this.Pattern.CurrentFillPatternColor });
            this.Properties.Add(new A11yPatternProperty() { Name = "FillPatternStyle ", Value = this.Pattern.CurrentFillPatternStyle });
            this.Properties.Add(new A11yPatternProperty() { Name = "Shape ", Value = this.Pattern.CurrentShape });
            this.Properties.Add(new A11yPatternProperty() { Name = "StyleId ", Value = this.Pattern.CurrentStyleId });
            this.Properties.Add(new A11yPatternProperty() { Name = "StyleName ", Value = this.Pattern.CurrentStyleName });
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
