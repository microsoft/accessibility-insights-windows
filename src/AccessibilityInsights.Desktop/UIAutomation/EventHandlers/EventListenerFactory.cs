// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Desktop.Types;
using Axe.Windows.Telemetry;
using Axe.Windows.Win32;
using System;
using System.Collections.Generic;
using System.Threading;
using UIAutomationClient;

namespace Axe.Windows.Desktop.UIAutomation.EventHandlers
{
    /// <summary>
    /// Maintain the AutomationEvent handlers for UI interaction
    /// since different sets of event handlers can be created for each purpose, each factory keeps its own copy of CUIAutomation object. 
    /// it will be released at disposal.
    /// </summary>
    public class EventListenerFactory:IDisposable
    {
        public FocusChangedEventListener EventListenerFocusChanged { get; private set; }
        public StructureChangedEventListener EventListenerStructureChanged { get; private set; }
        public PropertyChangedEventListener EventListenerPropertyChanged { get; private set; }
        public ChangesEventListener EventListenerChanges { get; private set; }
        public NotificationEventListener EventListenerNotification { get; private set; }
        public TextEditTextChangedEventListener EventListenerTextEditTextChanged { get; private set; }
        public ActiveTextPositionChangedEventListener EventListenerActiveTextPositionChanged { get; private set; }
        public Dictionary<int,EventListener> EventListeners { get; private set; }

        public TreeScope Scope { get; private set; }
        public CUIAutomation UIAutomation { get; private set; }
        public CUIAutomation8 UIAutomation8 { get; private set; }

        private readonly A11yElement RootElement;

        const int ThreadExitGracePeriod = 2; // 2 seconds

        #region Message Processing part
        /// <summary>
        /// Message Queue to keep the request from callers on other thread(s)
        /// </summary>
        private Queue<EventListenerFactoryMessage> _msgQueue = new Queue<EventListenerFactoryMessage>();

        private Thread _threadBackground = null;
        private AutoResetEvent _autoEventInit; // Event used to allow background thread to take any required initialization action.
        private AutoResetEvent _autoEventMsg; // Event used to notify the background thread that action is required.
        private AutoResetEvent _autoEventFinish; // Event used to notify the end of worker thread

        /// <summary>
        /// Start a worker thread to handle UIAutomation request on a dedicated thread
        /// </summary>
        private void StartWorkerThread()
        {
            // This sample doesn't expect to enter here with a background thread already initialized.
            if (_threadBackground != null)
            {
                return;
            }

            _autoEventFinish = new AutoResetEvent(false);
            // The main thread will notify the background thread later when it's time to close down.
            _autoEventMsg = new AutoResetEvent(false);

            // Create the background thread, and wait until it's ready to start working.
            _autoEventInit = new AutoResetEvent(false);
            ThreadStart threadStart = new ThreadStart(ProcessMessageQueue);

            _threadBackground = new Thread(threadStart);
            _threadBackground.SetApartmentState(ApartmentState.MTA); // The event handler must run on an MTA thread.
            _threadBackground.Start();

            _autoEventInit.WaitOne();
        }

        /// <summary>
        /// Worker method for private thread to process Message Queue
        /// </summary>
        private void ProcessMessageQueue()
        {
            this.UIAutomation = new CUIAutomation();

            // CUIAutomation8 was introduced in Windows 8, so don't try it on Windows 7.
            // Reference: https://msdn.microsoft.com/en-us/library/windows/desktop/hh448746(v=vs.85).aspx?f=255&MSPPError=-2147217396
            if (!NativeMethods.IsWindows7())
            {
                this.UIAutomation8 = new CUIAutomation8();
            }

            _autoEventInit.Set();

            // fCloseDown will be set true when the thread is to close down.
            bool fCloseDown = false;

            while (!fCloseDown)
            {
                // Wait here until we're told we have some work to do.
                _autoEventMsg.WaitOne();

                while (true)
                {
                    EventListenerFactoryMessage msgData;

                    // Note that none of the queue or message related action here is specific to UIA.
                    // Rather it is only a means for the main UI thread and the background MTA thread
                    // to communicate.

                    // Get a message from the queue of action-related messages.
                    lock (_msgQueue)
                    {
                        if(_msgQueue.Count != 0)
                        {
                            msgData = _msgQueue.Dequeue();
                        }
                        else
                        {
                            break;
                        }
                    }

                    switch(msgData.MessageType) 
                    {
                        case EventListenerFactoryMessageType.FinishThread:
                            // The main UI thread is telling this background thread to close down.
                            fCloseDown = true;
                            break;
                        case EventListenerFactoryMessageType.RegisterEventListener:
                            RegisterEventListener(msgData);
                            break;
                        case EventListenerFactoryMessageType.UnregisterEventListener:
                            UnregisterEventListener(msgData);
                            break;
                        case EventListenerFactoryMessageType.UnregisterAllEventListeners:
                            UnregisterAllEventListener();
                            break;
                    }

                    msgData.Processed();
                }
            }

            _autoEventFinish.Set();

        }

        private void UnregisterAllEventListener()
        {
            /// Need to find out a way to handle
            UnregisterEventListener(new EventListenerFactoryMessage
            {
                EventId = EventType.UIA_AutomationFocusChangedEventId
            });

            UnregisterEventListener(new EventListenerFactoryMessage
            {
                EventId = EventType.UIA_StructureChangedEventId
            });

            UnregisterEventListener(new EventListenerFactoryMessage
            {
                EventId = EventType.UIA_AutomationPropertyChangedEventId
            });

            UnregisterEventListener(new EventListenerFactoryMessage
            {
                EventId = EventType.UIA_NotificationEventId
            });

            UnregisterEventListener(new EventListenerFactoryMessage
            {
                EventId = EventType.UIA_TextEdit_TextChangedEventId
            });

            UnregisterEventListener(new EventListenerFactoryMessage
            {
                EventId = EventType.UIA_ActiveTextPositionChangedEventId
            });

            UnregisterEventListener(new EventListenerFactoryMessage
            {
                EventId = EventType.UIA_ChangesEventId
            });

            HandleUIAutomationEventMessage listener = null;
            try
            {
                foreach (var e in this.EventListeners.Values)
                {
                    listener = e.ListenEventMessage;
                    e.Dispose();
                }
                this.EventListeners.Clear();
                if (listener != null)
                {
                    var m = EventMessage.GetInstance(EventType.UIA_EventRecorderNotificationEventId, null);
                    m.Properties = new List<KeyValuePair<string, dynamic>>() { new KeyValuePair<string, dynamic>("Message", "Succeeded to unregister all event listeners.") };
                    listener(m);
                }
            }
            catch (Exception e)
            {
                e.ReportException();
                var m  = EventMessage.GetInstance(EventType.UIA_EventRecorderNotificationEventId, null);
                m.Properties = new List<KeyValuePair<string, dynamic>>() { new KeyValuePair<string, dynamic>("Message", $"Failed to unregister all listeners: {e.Message}") };
                listener(m);
            }
        }

        /// <summary>
        /// Process Unregister event message
        /// </summary>
        /// <param name="msgData"></param>
        private void UnregisterEventListener(EventListenerFactoryMessage msgData)
        {
            HandleUIAutomationEventMessage listener = null; 
            try
            {
                switch (msgData.EventId)
                {
                    case EventType.UIA_AutomationFocusChangedEventId:
                        if (this.EventListenerFocusChanged != null)
                        {
                            listener = this.EventListenerFocusChanged.ListenEventMessage;
                            this.EventListenerFocusChanged.Dispose();
                            this.EventListenerFocusChanged = null;
                        }
                        break;
                    case EventType.UIA_StructureChangedEventId:
                        if (this.EventListenerStructureChanged != null)
                        {
                            listener = this.EventListenerStructureChanged.ListenEventMessage;
                            this.EventListenerStructureChanged.Dispose();
                            this.EventListenerStructureChanged = null;
                        }
                        break;
                    case EventType.UIA_AutomationPropertyChangedEventId:
                        if (this.EventListenerPropertyChanged != null)
                        {
                            listener = this.EventListenerPropertyChanged.ListenEventMessage;
                            this.EventListenerPropertyChanged.Dispose();
                            this.EventListenerPropertyChanged = null;
                        }
                        break;
                    case EventType.UIA_TextEdit_TextChangedEventId:
                        if (this.EventListenerTextEditTextChanged != null)
                        {
                            listener = this.EventListenerTextEditTextChanged.ListenEventMessage;
                            this.EventListenerTextEditTextChanged.Dispose();
                            this.EventListenerTextEditTextChanged = null;
                        }
                        break;
                    case EventType.UIA_ChangesEventId:
                        if (this.EventListenerChanges != null)
                        {
                            listener = this.EventListenerChanges.ListenEventMessage;
                            this.EventListenerChanges.Dispose();
                            this.EventListenerChanges = null;
                        }
                        break;
                    case EventType.UIA_NotificationEventId:
                        if (this.EventListenerNotification != null)
                        {
                            listener = this.EventListenerNotification.ListenEventMessage;
                            this.EventListenerNotification.Dispose();
                            this.EventListenerNotification = null;
                        }
                        break;
                    case EventType.UIA_ActiveTextPositionChangedEventId:
                        if (this.EventListenerActiveTextPositionChanged != null)
                        {
                            listener = this.EventListenerActiveTextPositionChanged.ListenEventMessage;
                            this.EventListenerActiveTextPositionChanged.Dispose();
                            this.EventListenerActiveTextPositionChanged = null;
                        }
                        break;
                    default:
                        if (this.EventListeners.ContainsKey(msgData.EventId))
                        {
                            var l = this.EventListeners[msgData.EventId];
                            listener = l.ListenEventMessage;
                            this.EventListeners.Remove(msgData.EventId);
                            l.Dispose();
                        }
                        break;
                }

                if (listener != null)
                {
                    var m = EventMessage.GetInstance(EventType.UIA_EventRecorderNotificationEventId, null);
                    m.Properties = new List<KeyValuePair<string, dynamic>>() {
                        new KeyValuePair<string, dynamic>("Message", "Succeeded to unregister a event listeners"),
                        new KeyValuePair<string, dynamic>("Event Id", msgData.EventId),
                        new KeyValuePair<string, dynamic>("Event Name", EventType.GetInstance().GetNameById(msgData.EventId)),
                    };
                    listener(m);
                }
            }
            catch (Exception e)
            {
                e.ReportException();
                var m = EventMessage.GetInstance(EventType.UIA_EventRecorderNotificationEventId, null);
                m.Properties = new List<KeyValuePair<string, dynamic>>() {
                        new KeyValuePair<string, dynamic>("Message", "Failed to unregister a event listeners"),
                        new KeyValuePair<string, dynamic>("Event Id", msgData.EventId),
                        new KeyValuePair<string, dynamic>("Event Name", EventType.GetInstance().GetNameById(msgData.EventId)),
                        new KeyValuePair<string, dynamic>("Error", e.Message)
                    };

                listener(m);
                /// it is very unexpected situation. 
                /// need to figure out a way to prevent it or handle it more gracefully
            }
        }

        /// <summary>
        /// Process Register event message
        /// </summary>
        /// <param name="msgData"></param>
        private void RegisterEventListener(EventListenerFactoryMessage msgData)
        {
            try
            {
                EventMessage m = null;

                switch (msgData.EventId)
                {
                    case EventType.UIA_AutomationFocusChangedEventId:
                        if (this.EventListenerFocusChanged == null)
                        {
                            this.EventListenerFocusChanged = new FocusChangedEventListener(this.UIAutomation, msgData.Listener);
                        }
                        break;
                    case EventType.UIA_StructureChangedEventId:
                        if (this.EventListenerStructureChanged == null)
                        {
                            this.EventListenerStructureChanged = new StructureChangedEventListener(this.UIAutomation, this.RootElement.PlatformObject, this.Scope, msgData.Listener);
                        }
                        break;
                    case EventType.UIA_AutomationPropertyChangedEventId:
                        if (this.EventListenerPropertyChanged == null)
                        {
                            this.EventListenerPropertyChanged = new PropertyChangedEventListener(this.UIAutomation, this.RootElement.PlatformObject, this.Scope, msgData.Listener,msgData.Properties);
                        }
                        break;
                    case EventType.UIA_TextEdit_TextChangedEventId:
                        if (this.EventListenerTextEditTextChanged == null)
                        {
                            this.EventListenerTextEditTextChanged = new TextEditTextChangedEventListener(this.UIAutomation8, this.RootElement.PlatformObject, this.Scope, msgData.Listener);
                        }
                        break;
                    case EventType.UIA_ChangesEventId:
                        if (this.EventListenerChanges == null)
                        {
                            this.EventListenerChanges = new ChangesEventListener(this.UIAutomation8, this.RootElement.PlatformObject, this.Scope, msgData.Listener);
                        }
                        break;
                    case EventType.UIA_NotificationEventId:
                        if (NativeMethods.IsWindowsRS3OrLater())
                        {
                            if (this.EventListenerNotification == null)
                            {
                                this.EventListenerNotification = new NotificationEventListener(this.UIAutomation8, this.RootElement.PlatformObject, this.Scope, msgData.Listener);
                            }
                        }
                        else
                        {
                            m = EventMessage.GetInstance(EventType.UIA_EventRecorderNotificationEventId, null);
                            m.Properties = new List<KeyValuePair<string, dynamic>>() {
                                new KeyValuePair<string, dynamic>("Message", "Event listener registration is rejected."),
                                new KeyValuePair<string, dynamic>("Event Id", msgData.EventId),
                                new KeyValuePair<string, dynamic>("Event Name", EventType.GetInstance().GetNameById(msgData.EventId)),
                                new KeyValuePair<string, dynamic>("Reason", "Not supported platform"),
                            };
                            msgData.Listener(m);
                        }
                        break;
                    case EventType.UIA_ActiveTextPositionChangedEventId:
                        if (NativeMethods.IsWindowsRS5OrLater())
                        {
                            if (this.EventListenerNotification == null)
                            {
                                this.EventListenerActiveTextPositionChanged = new ActiveTextPositionChangedEventListener(this.UIAutomation8, this.RootElement.PlatformObject, this.Scope, msgData.Listener);
                            }
                        }
                        else
                        {
                            m = EventMessage.GetInstance(EventType.UIA_EventRecorderNotificationEventId, null);
                            m.Properties = new List<KeyValuePair<string, dynamic>>() {
                                new KeyValuePair<string, dynamic>("Message", "Event listener registration is rejected."),
                                new KeyValuePair<string, dynamic>("Event Id", msgData.EventId),
                                new KeyValuePair<string, dynamic>("Event Name", EventType.GetInstance().GetNameById(msgData.EventId)),
                                new KeyValuePair<string, dynamic>("Reason", "Not supported platform"),
                            };
                            msgData.Listener(m);
                        }
                        break;
                    default:
                        if (this.EventListeners.ContainsKey(msgData.EventId) == false)
                        {
                            this.EventListeners.Add(msgData.EventId, new EventListener(this.UIAutomation, this.RootElement.PlatformObject, this.Scope, msgData.EventId, msgData.Listener));
                        }
                        break;
                }

                m = EventMessage.GetInstance(EventType.UIA_EventRecorderNotificationEventId, null);
                m.Properties = new List<KeyValuePair<string, dynamic>>() {
                        new KeyValuePair<string, dynamic>("Message", "Succeeded to register an event listener"),
                        new KeyValuePair<string, dynamic>("Event Id", msgData.EventId),
                        new KeyValuePair<string, dynamic>("Event Name", EventType.GetInstance().GetNameById(msgData.EventId)),
                    };
                msgData.Listener(m);
                if(msgData.EventId == EventType.UIA_AutomationFocusChangedEventId)
                {
                    this.EventListenerFocusChanged.ReadyToListen = true;
                }
            }
            catch (Exception e)
            {
                e.ReportException();
                var m = EventMessage.GetInstance(EventType.UIA_EventRecorderNotificationEventId, null);
                m.Properties = new List<KeyValuePair<string, dynamic>>() {
                        new KeyValuePair<string, dynamic>("Message", "Failed to register an event listener"),
                        new KeyValuePair<string, dynamic>("Event Id", msgData.EventId),
                        new KeyValuePair<string, dynamic>("Event Name", EventType.GetInstance().GetNameById(msgData.EventId)),
                        new KeyValuePair<string, dynamic>("Error", e.Message)
                };
                msgData.Listener(m);
            }
        }

        /// <summary>
        /// Request Worker thread finish and wait for 2 seconds to finish. otherwise, there will be an exception. 
        /// </summary>
        private void FinishWorkerThread()
        {
            AddMessageToQueue(new EventListenerFactoryMessage() { MessageType = EventListenerFactoryMessageType.FinishThread });
            _autoEventFinish.WaitOne(TimeSpan.FromSeconds(ThreadExitGracePeriod));
            if (this._threadBackground.IsAlive)
            {
                this._threadBackground.Abort();
            }
        }

        /// <summary>
        /// Add a new Factory Message 
        /// </summary>
        /// <param name="msgData"></param>
        private void AddMessageToQueue(EventListenerFactoryMessage msgData)
        {
            // Request the lock, and block until it is obtained.
            lock(_msgQueue)
            { 
                // When the lock is obtained, add an element.
                _msgQueue.Enqueue(msgData);
            }

            _autoEventMsg.Set();
        }
        #endregion

        public EventListenerFactory(A11yElement rootElement) : this(rootElement, TreeScope.TreeScope_Subtree) { }

        /// <summary>
        /// Cosntructor. 
        /// </summary>
        /// <param name="peDelegate"></param>
        /// <param name="rootElement">can be null but it is only for global events like focusChanged</param>
        /// <param name="scope"></param>
        public EventListenerFactory(A11yElement rootElement, TreeScope scope)
        {
            this.RootElement = rootElement;
            this.Scope = scope;
            this.EventListeners = new Dictionary<int, EventListener>();
            //Start worker thread
            StartWorkerThread();
        }

        /// <summary>
        /// Register a single event listener
        /// in case of property listening, since it is monolithic, you need to stop existing property listener first. 
        /// the implicit cleanup is not defined.
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="peDelegate"></param>
        /// <param name="properties">required only for PropertyChanged Event listening</param>
        public void RegisterAutomationEventListener(int eventId, HandleUIAutomationEventMessage peDelegate, int[] properties = null)
        {
            AddMessageToQueue(new EventListenerFactoryMessage()
            {
                MessageType = EventListenerFactoryMessageType.RegisterEventListener,
                Listener = peDelegate,
                EventId = eventId,
                Properties = properties
            });
        }

        /// <summary>
        /// Unregister a automation event listener
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="wait">wait to complete</param>
        public void UnregisterAutomationEventListener(int eventId,bool wait = false)
        {
            var msg = new EventListenerFactoryMessage()
            {
                MessageType = EventListenerFactoryMessageType.UnregisterEventListener,
                EventId = eventId
            };

            AddMessageToQueue(msg);

            if (wait)
            {
                msg.WaitForProcessed(2000);// expect to be done in 2 seconds.
            }
        }

        /// <summary>
        /// Unregister all event listeners
        /// </summary>
        public void UnregisterAllAutomationEventListners()
        {
            var msg = new EventListenerFactoryMessage()
            {
                MessageType = EventListenerFactoryMessageType.UnregisterAllEventListeners,
            };

            AddMessageToQueue(msg);

            msg.WaitForProcessed(2000);// expect to be done in 2 seconds.

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    UnregisterAllAutomationEventListners();
                    FinishWorkerThread();
                    if (_autoEventInit != null)
                    {
                        _autoEventInit.Dispose();
                        _autoEventInit = null;
                    }
                    if (_autoEventFinish != null)
                    {
                        _autoEventFinish.Dispose(); ;
                        _autoEventFinish = null;
                    }
                    if (_autoEventMsg != null)
                    {
                        _autoEventMsg.Dispose();
                        _autoEventMsg = null;
                    }
                }

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AutomationEventHandlerFactory() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    #region private classes for messaging
    /// <summary>
    /// Message Type enum to support communication between UI Thread and Listener Factory Thread
    /// </summary>
    public enum EventListenerFactoryMessageType
    {
        Null,
        RegisterEventListener,
        UnregisterEventListener,
        UnregisterAllEventListeners,
        FinishThread
    };

    /// <summary>
    /// EventListener Message data
    /// it contains message type and other information for further action
    /// </summary>
    public class EventListenerFactoryMessage:IDisposable
    {
        internal EventListenerFactoryMessageType MessageType;
        internal int EventId;
        internal HandleUIAutomationEventMessage Listener;
        internal int[] Properties;

        private AutoResetEvent _autoEventProcessed; // set when the message is processed.

        public EventListenerFactoryMessage()
        {
            this._autoEventProcessed = new AutoResetEvent(false);
        }

        /// <summary>
        /// Wait until message is processed.
        /// if not processed in given time, exception will be thrown. 
        /// </summary>
        /// <param name="milliseconds"></param>
        public void WaitForProcessed(int milliseconds)
        {
            this._autoEventProcessed.WaitOne(milliseconds);
        }

        /// <summary>
        /// signal to finish processing this message.
        /// </summary>
        internal void Processed()
        {
            this._autoEventProcessed.Set();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._autoEventProcessed.Dispose();
                    this._autoEventProcessed = null;
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    };
    #endregion
}
