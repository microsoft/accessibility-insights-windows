// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using Axe.Windows.Core.Bases;
using UIAutomationClient;
using Axe.Windows.Core.Attributes;

using static System.FormattableString;

namespace Axe.Windows.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for MultipleView Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671282(v=vs.85).aspx
    /// </summary>
    public class MultipleViewPattern : A11yPattern
    {
        IUIAutomationMultipleViewPattern Pattern = null;

        public MultipleViewPattern(A11yElement e, IUIAutomationMultipleViewPattern p) : base(e, PatternType.UIA_MultipleViewPatternId)
        {
            Pattern = p;

            PopulateProperties();
        }

        private void PopulateProperties()
        {
            this.Properties.Add(new A11yPatternProperty() { Name = "CurrentView", Value = this.Pattern.CurrentCurrentView });
            var array = this.Pattern.GetCurrentSupportedViews();
            if (array.Length > 0)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    var view = (int) array.GetValue(i);
                    Properties.Add(new A11yPatternProperty() { Name = Invariant($"SupportedViews[{i}]"), Value = Invariant($"{view}: {Pattern.GetViewName(view)}")});
                }
            }

        }

        [PatternMethod]
        public void SetCurrentView(int view)
        {
            this.Pattern.SetCurrentView(view);
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
