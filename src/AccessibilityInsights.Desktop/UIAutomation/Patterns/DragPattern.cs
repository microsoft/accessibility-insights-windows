// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Axe.Windows.Core.Bases;
using UIAutomationClient;
using Axe.Windows.Core.Attributes;
using Axe.Windows.Desktop.Misc;
using Axe.Windows.Desktop.Types;
using System.Runtime.InteropServices;

using static System.FormattableString;

namespace Axe.Windows.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for Drag Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/hh707348(v=vs.85).aspx
    /// </summary>
    [PatternEvent(Id = EventType.UIA_Drag_DragStartEventId)]
    [PatternEvent(Id = EventType.UIA_Drag_DragCancelEventId)]
    [PatternEvent(Id = EventType.UIA_Drag_DragCompleteEventId)]
    public class DragPattern : A11yPattern
    {
        IUIAutomationDragPattern Pattern = null;

        public DragPattern(A11yElement e, IUIAutomationDragPattern p) : base(e, PatternType.UIA_DragPatternId)
        {
            Pattern = p;

            // though member method is not action specific, this pattern means for UI action. 
            this.IsUIActionable = true;

            PopulateProperties();
        }

        private void PopulateProperties()
        {
            this.Properties.Add(new A11yPatternProperty() { Name = "DropEffect", Value = this.Pattern.CurrentDropEffect });
            this.Properties.Add(new A11yPatternProperty() { Name = "DropEffects", Value = GetDropEffectsString(this.Pattern.CurrentDropEffects) });
            this.Properties.Add(new A11yPatternProperty() { Name = "IsGrabbed", Value = Convert.ToBoolean(this.Pattern.CurrentIsGrabbed) });
        }

        private static dynamic GetDropEffectsString(Array effects)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");

            if (effects.Length > 0)
            {
                sb.Append(effects.GetValue(0));
                for (int i = 1; i < effects.Length; i++)
                {
                    sb.Append(Invariant($", {effects.GetValue(i)}"));
                }
            }
            sb.Append("]");

            return sb.ToString();
        }

        [PatternMethod]
        public List<DesktopElement> GetGrabbedItems()
        {
            return this.Pattern.GetCurrentGrabbedItems()?.ToListOfDesktopElements();      
        }

        protected override void Dispose(bool disposing)
        {
            if (Pattern != null)
            {
                Marshal.ReleaseComObject(Pattern);
                this.Pattern = null;
            }

            base.Dispose(disposing);
        }
    }
}

