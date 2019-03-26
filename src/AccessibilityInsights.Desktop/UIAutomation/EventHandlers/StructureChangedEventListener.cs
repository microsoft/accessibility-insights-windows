// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Misc;
using AccessibilityInsights.Desktop.Types;
using System.Collections.Generic;
using UIAutomationClient;

namespace AccessibilityInsights.Desktop.UIAutomation.EventHandlers
{

    public class StructureChangedEventListener : EventListenerBase, IUIAutomationStructureChangedEventHandler
    {
        /// <summary>
        /// Create an event handler and register it.
        /// </summary>
        public StructureChangedEventListener(CUIAutomation uia, IUIAutomationElement element, TreeScope scope, HandleUIAutomationEventMessage peDelegate) : base(uia,element, scope, EventType.UIA_StructureChangedEventId, peDelegate)
        {
            Init();
        }

        public override void Init()
        {
            IUIAutomation uia = this.IUIAutomation;
            if (uia != null)
            {
                uia.AddStructureChangedEventHandler(this.Element, this.Scope, null, this);
                this.IsHooked = true;
            }
        }

        public void HandleStructureChangedEvent(IUIAutomationElement sender, StructureChangeType changeType, int[] runtimeId)
        {
            var m = EventMessage.GetInstance(EventType.UIA_StructureChangedEventId, sender);
            if (m != null)
            {
                m.Properties.Add(new KeyValuePair<string, dynamic>("StructureChangeType", changeType));
                m.Properties.Add(new KeyValuePair<string, dynamic>("Runtime Id", runtimeId.ConvertInt32ArrayToString()));
                this.ListenEventMessage(m);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this.IsHooked)
                    {
                        IUIAutomation uia = this.IUIAutomation;
                        if (uia != null)
                        {
                            uia.RemoveStructureChangedEventHandler(this.Element, this);
                        }
                    }
                }
            }
            base.Dispose(disposing);
        }
    }
}
