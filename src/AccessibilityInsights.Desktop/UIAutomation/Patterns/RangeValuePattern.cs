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
    /// Control pattern wrapper for RangeValue Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671283(v=vs.85).aspx
    /// </summary>
    public class RangeValuePattern : A11yPattern
    {
        IUIAutomationRangeValuePattern Pattern = null;

        public RangeValuePattern(A11yElement e, IUIAutomationRangeValuePattern p) : base(e, PatternType.UIA_RangeValuePatternId)
        {
            Pattern = p;

            PopulateProperties();
        }

        private void PopulateProperties()
        {
            this.Properties.Add(new A11yPatternProperty() { Name = "IsReadOnly", Value = Convert.ToBoolean(this.Pattern.CurrentIsReadOnly) });
            this.Properties.Add(new A11yPatternProperty() { Name = "LargeChange", Value = this.Pattern.CurrentLargeChange });
            this.Properties.Add(new A11yPatternProperty() { Name = "Maximum", Value = this.Pattern.CurrentMaximum });
            this.Properties.Add(new A11yPatternProperty() { Name = "Minimum", Value = this.Pattern.CurrentMinimum });
            this.Properties.Add(new A11yPatternProperty() { Name = "SmallChange", Value = this.Pattern.CurrentSmallChange });
            this.Properties.Add(new A11yPatternProperty() { Name = "Value", Value = this.Pattern.CurrentValue });
        }

        [PatternMethod(IsUIAction = true)]
        public void SetValue(double val)
        {
            this.Pattern.SetValue(val);
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

