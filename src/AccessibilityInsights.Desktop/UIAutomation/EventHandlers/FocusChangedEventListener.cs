// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Desktop.Types;
using System;
using System.Threading;
using UIAutomationClient;

namespace Axe.Windows.Desktop.UIAutomation.EventHandlers
{
    public class FocusChangedEventListener:IDisposable, IUIAutomationFocusChangedEventHandler
    {
        public HandleUIAutomationEventMessage ListenEventMessage { get; private set; }

        /// <summary>
        /// indicate whether event handler is hooked or not. 
        /// </summary>
        bool IsHooked { get; set; }

        /// <summary>
        /// indicate whether listener is ready to process the data
        /// </summary>
        public bool ReadyToListen { get; set; } = false;

        private CUIAutomation UIAutomation = null;

        public FocusChangedEventListener(CUIAutomation uia, HandleUIAutomationEventMessage peDelegate)
        {
            this.UIAutomation = uia;
            this.ListenEventMessage = peDelegate;
            this.UIAutomation.AddFocusChangedEventHandler(null,this);
            IsHooked = true;
        }

        public void HandleFocusChangedEvent(IUIAutomationElement sender)
        {
            if (ReadyToListen)
            {
                var m = EventMessage.GetInstance(EventType.UIA_AutomationFocusChangedEventId, sender);
                if (m != null)
                {
                    ListenEventMessage(m);
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (IsHooked && this.UIAutomation != null)
                {
                    try
                    {
                        this.UIAutomation.RemoveFocusChangedEventHandler(this);
                        this.UIAutomation = null;
                    }
                    catch(ThreadAbortException)
                    {
                        // silently ignore exception since a ThreadAbortException is thrown at the process exit.
                    }
                }

                disposedValue = true;
            }
        }

        ~FocusChangedEventListener()
        {
            Dispose(false);
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
