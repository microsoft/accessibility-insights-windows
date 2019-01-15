// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Desktop.Types;
using System.Collections.Generic;
using UIAutomationClient;

namespace AccessibilityInsights.Desktop.UIAutomation.EventHandlers
{
    /// <summary>
    /// NotificationEvent listener. 
    /// this event is available from Win10 RS3
    /// </summary>
    public class NotificationEventListener : EventListenerBase, IUIAutomationNotificationEventHandler
    {
        /// <summary>
        /// Create an event handler and register it.
        /// </summary>
        public NotificationEventListener(CUIAutomation8 uia8, IUIAutomationElement element, TreeScope scope, HandleUIAutomationEventMessage peDelegate) : base(uia8, element, scope, EventType.UIA_NotificationEventId, peDelegate)
        {
            Init();
        }

        public override void Init()
        {
            IUIAutomation5 uia5 = this.IUIAutomation5;
            if (uia5 != null)
            {
                uia5.AddNotificationEventHandler(this.Element, this.Scope, null, this);
                this.IsHooked = true;
            }
        }

        public void HandleNotificationEvent(IUIAutomationElement sender, NotificationKind kind, NotificationProcessing process, string displayString, string activityId)
        {
            var m = EventMessage.GetInstance(this.EventId, sender);

            if (m != null)
            {
                m.Properties = new List<KeyValuePair<string, dynamic>>
                {
                    new KeyValuePair<string, dynamic>("NotificationKind", kind.ToString()),
                    new KeyValuePair<string, dynamic>("NotificationProcessing", process.ToString()),
                    new KeyValuePair<string, dynamic>("Display", displayString),
                    new KeyValuePair<string, dynamic>("ActivityId", activityId),
                };
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
                        IUIAutomation5 uia5 = this.IUIAutomation5;
                        if (uia5 != null)
                        {
                            uia5.RemoveNotificationEventHandler(this.Element, this);
                        }
                    }
                }
            }
            base.Dispose(disposing);
        }
    }
}
