// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using UIAutomationClient;

namespace AccessibilityInsights.Desktop.UIAutomation.EventHandlers
{
    /// <summary>
    /// Indicate the message from Event
    /// </summary>
    /// <param name="txt"></param>
    public delegate void HandleUIAutomationEventMessage(EventMessage em);

    public abstract class EventListenerBase:IDisposable
    {
        public int EventId { get; private set; }
        public IUIAutomationElement Element { get; private set; }

        public HandleUIAutomationEventMessage ListenEventMessage { get; private set; }
        public TreeScope Scope { get; private set; }
        public bool IsHooked { get; protected set; }
        private CUIAutomation UIAutomation;
        private CUIAutomation8 UIAutomation8;

        protected IUIAutomation IUIAutomation => UIAutomation as IUIAutomation;
        protected IUIAutomation4 IUIAutomation4 => UIAutomation8 as IUIAutomation4;
        protected IUIAutomation5 IUIAutomation5 => UIAutomation8 as IUIAutomation5;
        protected IUIAutomation6 IUIAutomation6 => UIAutomation8 as IUIAutomation6;

        /// <summary>
        /// Ctor to create an event handler (with CUIAutomation8) and register it.
        /// </summary>
        public EventListenerBase(CUIAutomation8 uia8, IUIAutomationElement element, TreeScope scope, int eventId, HandleUIAutomationEventMessage peDelegate)
            : this(element, scope, eventId, peDelegate)
        {
            this.UIAutomation8 = uia8;
        }

        /// <summary>
        /// Ctor to create an event handler (with CUIAutomation) and register it.
        /// </summary>
        public EventListenerBase(CUIAutomation uia, IUIAutomationElement element, TreeScope scope, int eventId, HandleUIAutomationEventMessage peDelegate)
            : this (element, scope, eventId, peDelegate)
        {
            this.UIAutomation = uia;
        }

        /// <summary>
        /// Private ctor to set common fields
        /// </summary>
        private EventListenerBase(IUIAutomationElement element, TreeScope scope, int eventId, HandleUIAutomationEventMessage peDelegate)
        {
            this.EventId = eventId;
            this.Element = element;
            this.ListenEventMessage = peDelegate;
            this.Scope = scope;
        }

        /// <summary>
        /// Initialize event handler
        /// it should be called in side of derived class constructor.
        /// </summary>
        public virtual void Init()
        {
        }

        #region IDisposable Support
        protected bool disposedValue { get; private set; } // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (disposing && this.IsHooked)
                    {
                        UIAutomation = null;
                        UIAutomation8 = null;
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
