// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using UIAutomationClient;

namespace Axe.Windows.Desktop.UIAutomation.EventHandlers
{
    /// <summary>
    /// Generic UIAutomation event handler wrapper
    /// </summary>
    public class EventListener: EventListenerBase, IUIAutomationEventHandler
    {
        /// <summary>
        /// Create an event handler and register it.
        /// </summary>
        public EventListener(CUIAutomation uia, IUIAutomationElement element,TreeScope scope, int eventId,HandleUIAutomationEventMessage peDelegate):base(uia, element,scope,eventId,peDelegate)
        {
            Init();
        }

        public override void Init()
        {
            IUIAutomation uia = this.IUIAutomation;
            if (uia != null)
            {
                uia.AddAutomationEventHandler(this.EventId, this.Element, this.Scope, null, this);
                this.IsHooked = true;
            }
        }

        public void HandleAutomationEvent(IUIAutomationElement sender, int eventId)
        {
            var m = EventMessage.GetInstance(eventId, sender);
            if (m != null)
            {
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
                            uia.RemoveAutomationEventHandler(this.EventId, this.Element, this);
                        }
                    }
                }
            }
            base.Dispose(disposing);
        }
    }
}
