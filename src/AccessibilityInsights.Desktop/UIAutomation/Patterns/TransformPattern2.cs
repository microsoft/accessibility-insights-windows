// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using System;
using AccessibilityInsights.Core.Bases;
using UIAutomationClient;
using AccessibilityInsights.Core.Attributes;

namespace AccessibilityInsights.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for Transform2 Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671291(v=vs.85).aspx
    /// </summary>
    public class TransformPattern2 : A11yPattern
    {
        IUIAutomationTransformPattern2 Pattern = null;

        public TransformPattern2(A11yElement e, IUIAutomationTransformPattern2 p) : base(e, PatternType.UIA_TransformPattern2Id)
        {
            Pattern = p;

            PopulateProperties();
        }

        private void PopulateProperties()
        {
            this.Properties.Add(new A11yPatternProperty() { Name = "CanMove", Value = Convert.ToBoolean(this.Pattern.CurrentCanMove) });
            this.Properties.Add(new A11yPatternProperty() { Name = "CanResize", Value = Convert.ToBoolean(this.Pattern.CurrentCanResize) });
            this.Properties.Add(new A11yPatternProperty() { Name = "CanRotate", Value = Convert.ToBoolean(this.Pattern.CurrentCanRotate) });
            this.Properties.Add(new A11yPatternProperty() { Name = "CanZoom", Value = Convert.ToBoolean(this.Pattern.CurrentCanZoom) });
            this.Properties.Add(new A11yPatternProperty() { Name = "CanZoomLevel", Value = Convert.ToBoolean(this.Pattern.CurrentZoomLevel) });
            this.Properties.Add(new A11yPatternProperty() { Name = "CanZoomMaximum", Value = Convert.ToBoolean(this.Pattern.CurrentZoomMaximum) });
            this.Properties.Add(new A11yPatternProperty() { Name = "CanZoomMinimum", Value = Convert.ToBoolean(this.Pattern.CurrentZoomMinimum) });
        }

        [PatternMethod(IsUIAction = true)]
        public void Zoom(double zoomValue)
        {
            this.Pattern.Zoom(zoomValue);
        }

        [PatternMethod(IsUIAction = true)]
        public void ZoomByUnit(ZoomUnit zu)
        {
            this.Pattern.ZoomByUnit(zu);
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
