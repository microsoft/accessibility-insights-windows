// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Desktop.Types;
using System.Collections.Generic;
using UIAutomationClient;

namespace AccessibilityInsights.Desktop.UIAutomation.EventHandlers
{
    public class PropertyChangedEventListener : EventListenerBase, IUIAutomationPropertyChangedEventHandler
    {
        readonly int[] propertyArray = null;

        /// <summary>
        /// Create an event handler and register it.
        /// </summary>
        public PropertyChangedEventListener(CUIAutomation uia,IUIAutomationElement element, TreeScope scope, HandleUIAutomationEventMessage peDelegate, int[] properties) : base(uia,element, scope, EventType.UIA_AutomationPropertyChangedEventId, peDelegate)
        {
            this.propertyArray = properties;
            Init();
        }

        public override void Init()
        {
            IUIAutomation uia = this.IUIAutomation;
            if (uia != null)
            {
                uia.AddPropertyChangedEventHandler(this.Element, this.Scope, null, this, this.propertyArray);
                this.IsHooked = true;
            }
        }

        public void HandlePropertyChangedEvent(IUIAutomationElement sender, int propertyId, object newValue)
        {
            var m = EventMessage.GetInstance(this.EventId, sender);

            if (m != null)
            {
                m.Properties.Add(new KeyValuePair<string, dynamic>("Property Id", propertyId));
                m.Properties.Add(new KeyValuePair<string, dynamic>("Property Name", PropertyType.GetInstance().GetNameById(propertyId)));

                if (newValue != null)
                {
                    m.Properties.Add(new KeyValuePair<string, dynamic>(newValue.GetType().Name, newValue));
                }

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
                            uia.RemovePropertyChangedEventHandler(this.Element, this);
                        }
                    }
                }
            }
            base.Dispose(disposing);
        }
    }
}
