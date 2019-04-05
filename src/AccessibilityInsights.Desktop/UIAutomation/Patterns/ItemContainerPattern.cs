// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using Axe.Windows.Core.Bases;
using UIAutomationClient;
using Axe.Windows.Core.Attributes;

namespace Axe.Windows.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for Item Container Control Pattern
    /// </summary>
    public class ItemContainerPattern : A11yPattern
    {
        IUIAutomationItemContainerPattern Pattern = null;
        private IUIAutomationElement UIAElement;

        public ItemContainerPattern(A11yElement e, IUIAutomationItemContainerPattern p) : base(e, PatternType.UIA_ItemContainerPatternId)
        {
            Pattern = p;
            this.UIAElement = e.PlatformObject;
        }

        [PatternMethod]
        public DesktopElement FindItemByProperty(int propertyId, object value)
        {
            return new DesktopElement(this.Pattern.FindItemByProperty(this.UIAElement, propertyId, value));
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
