// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Misc;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Desktop.Types;
using AccessibilityInsights.Desktop.UIAutomation.EventHandlers;
using System;

namespace AccessibilityInsights.Actions.Trackers
{
    /// <summary>
    /// Class FocusSelector
    /// </summary>
    public class FocusTracker : BaseTracker
    {
        /// <summary>
        /// Event Handler
        /// </summary>
        EventListenerFactory EventHandler = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action"></param>
        public FocusTracker(Action<A11yElement> action) : base(action)
        {
            this.EventHandler = new EventListenerFactory(null); // listen for all element. it works only for FocusChangedEvent
        }

        /// <summary>
        /// Stop or Pause
        /// </summary>
        public override void Stop()
        {
            if (this.EventHandler != null && IsStarted)
            {
                this.EventHandler.UnregisterAutomationEventListener(EventType.UIA_AutomationFocusChangedEventId);
                this.IsStarted = false;
            }
            base.Stop();
        }

        /// <summary>
        /// Start or Resume
        /// </summary>
        public override void Start()
        {
            if (IsStarted == false)
            {
                this.EventHandler?.RegisterAutomationEventListener(EventType.UIA_AutomationFocusChangedEventId, this.onFocusChangedEventForSelectingElement);
                IsStarted = true;
            }
        }

        /// <summary>
        /// Handle Focus Change event
        /// </summary>
        /// <param name="message"></param>

        private void onFocusChangedEventForSelectingElement(EventMessage message)
        {
            // only when focus is chosen for hilight
            if (message.EventId == EventType.UIA_AutomationFocusChangedEventId)
            {
                // exclude tooltip since it is transient UI. 
                if (IsStarted && message.Element != null)
                {
                    var element = GetElementBasedOnScope(message.Element);

                    if( element != null && element.IsRootElement() == false
                        && element.ControlTypeId != ControlType.UIA_ToolTipControlTypeId
                        && element.IsSameUIElement(this.SelectedElementRuntimeId, this.SelectedBoundingRectangle, this.SelectedControlTypeId, this.SelectedName) == false)
                    {
                        this.SelectedElementRuntimeId = element.RuntimeId;
                        this.SelectedBoundingRectangle = element.BoundingRectangle;
                        this.SelectedControlTypeId = element.ControlTypeId;
                        this.SelectedName = element.Name;
                        this.SetElement?.Invoke(element);
                    }
                }
                else
                {
                    message.Dispose();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (this.EventHandler != null)
            {
                this.EventHandler.Dispose();
                this.EventHandler = null;
            }
            base.Dispose(disposing);
        }
    }
}
