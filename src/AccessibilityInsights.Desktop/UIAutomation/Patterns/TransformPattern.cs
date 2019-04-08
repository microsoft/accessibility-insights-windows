// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using System;
using System.Linq;
using Axe.Windows.Core.Bases;
using UIAutomationClient;
using Axe.Windows.Core.Attributes;

namespace Axe.Windows.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for Transform Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671291(v=vs.85).aspx
    /// </summary>
    public class TransformPattern : A11yPattern
    {
        IUIAutomationTransformPattern Pattern = null;

        public TransformPattern(A11yElement e, IUIAutomationTransformPattern p) : base(e, PatternType.UIA_TransformPatternId)
        {
            Pattern = p;

            PopulateProperties();

            // Get UI Actionability based on Properties than methods.
            this.IsUIActionable = this.Properties.Any(pp => pp.Value == true);
        }

        private void PopulateProperties()
        {
            this.Properties.Add(new A11yPatternProperty() { Name = "CanMove", Value = Convert.ToBoolean(this.Pattern.CurrentCanMove) });
            this.Properties.Add(new A11yPatternProperty() { Name = "CanResize", Value = Convert.ToBoolean(this.Pattern.CurrentCanResize) });
            this.Properties.Add(new A11yPatternProperty() { Name = "CanRotate", Value = Convert.ToBoolean(this.Pattern.CurrentCanRotate) });
        }

        [PatternMethod]
        public void Move(double x, double y)
        {
            this.Pattern.Move(x, y);
        }

        [PatternMethod]
        public void Resize(double width, double height)
        {
            this.Pattern.Resize(width, height);
        }

        [PatternMethod]
        public void Rotate(double degree)
        {
            this.Pattern.Rotate(degree);
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
