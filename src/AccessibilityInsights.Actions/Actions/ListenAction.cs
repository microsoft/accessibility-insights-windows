// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Actions.Attributes;
using Axe.Windows.Actions.Contexts;
using Axe.Windows.Actions.Enums;
using Axe.Windows.Core.Bases;
using Axe.Windows.Desktop.Settings;
using Axe.Windows.Desktop.Types;
using Axe.Windows.Desktop.UIAutomation.EventHandlers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Axe.Windows.Actions
{
    /// <summary>
    /// Class LstenAction
    /// to listen and record events from elements
    /// </summary>
    [InteractionLevel(UxInteractionLevel.NoUxInteraction)]
    public class ListenAction:IDisposable
    {
        RecorderSetting Config { get; set; }
        ElementContext ElementContext { get; set; }
        EventListenerFactory EventListener { get; set; }
        /// <summary>
        /// External event listener. it should be called if it is not null.
        /// </summary>
        public HandleUIAutomationEventMessage ExternalListener { get; private set; }

        public Guid Id { get; private set; }

        bool IsRunning = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="ec"></param>
        /// <param name="listener"></param>
        private ListenAction(RecorderSetting config, ElementContext ec, HandleUIAutomationEventMessage listener)
        {
            this.Id = Guid.NewGuid();
            this.Config = config;
            this.ElementContext = ec;
            this.EventListener = new EventListenerFactory(ec.Element, config.ListenScope);
            this.ExternalListener = listener;
        }

        /// <summary>
        /// Start recording events
        /// </summary>
        public void Start()
        {
            this.IsRunning = true;
            InitFocusChangedEventListener();
            InitIndividualEventListeners();
            InitPropertyChangeListener();
        }

        /// <summary>
        /// Start Property change Event listener
        /// </summary>
        private void InitPropertyChangeListener()
        {
            var list = from c in this.Config.Properties
                       where this.Config.IsListeningAllEvents || c.CheckedCount > 0
                       select c.Id;

            if (list.Count() != 0)
            {
                this.EventListener.RegisterAutomationEventListener(EventType.UIA_AutomationPropertyChangedEventId, this.onEventFired, list.ToArray());
            }
        }

        /// <summary>
        /// Start Individual Event Listener
        /// </summary>
        private void InitIndividualEventListeners()
        {
            var list = from c in this.Config.Events
                       where this.Config.IsListeningAllEvents || c.CheckedCount > 0
                       select c;

            foreach (var l in list)
            {
                if (l.Id == EventType.UIA_StructureChangedEventId)
                {
                    this.EventListener.RegisterAutomationEventListener(EventType.UIA_StructureChangedEventId,this.onEventFired);
                }
                else
                {
                    this.EventListener.RegisterAutomationEventListener(l.Id, this.onEventFired);
                }
            }
        }

        /// <summary>
        /// Start Listening FocusChange events
        /// </summary>
        private void InitFocusChangedEventListener()
        {
            if(this.Config.IsListeningFocusChangedEvent)
            {
                this.EventListener.RegisterAutomationEventListener(EventType.UIA_AutomationFocusChangedEventId,this.onEventFired);
            }
        }

        /// <summary>
        /// request stopping event recording and wait until it is fully done.
        /// </summary>
        public void Stop()
        {
            IsRunning = false;
            this.EventListener.UnregisterAllAutomationEventListners();
        }

        /// <summary>
        /// Listener for fired event
        /// need to be thread safe to add element in EventLogs
        /// </summary>
        /// <param name="message"></param>
        private void onEventFired(EventMessage message)
        {
            if (IsRunning)
            {
                this.ExternalListener?.Invoke(message);
            }
        }

        #region static members
        /// <summary>
        /// Dictionary of all live listenAction instances
        /// </summary>
        static Dictionary<Guid, ListenAction> sListenActions = new Dictionary<Guid, ListenAction>();

        /// <summary>
        /// Create new Instance of ListenAction
        /// </summary>
        /// <param name="config"></param>
        /// <param name="ecId"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public static Guid CreateInstance(RecorderSetting config, Guid ecId, HandleUIAutomationEventMessage listener)
        {
            var ec = DataManager.GetDefaultInstance().GetElementContext(ecId);
            var la = new ListenAction(config, ec, listener);

            sListenActions.Add(la.Id, la);

            return la.Id;
        }

        /// <summary>
        /// Get ListenAction instance
        /// </summary>
        /// <param name="laId">ListenAction Id</param>
        /// <returns></returns>
        public static ListenAction GetInstance(Guid laId)
        {
            return sListenActions[laId];
        }

        /// <summary>
        /// Release Listen Action
        /// </summary>
        /// <param name="laId"></param>
        public static void ReleaseInstance(Guid laId)
        {
            var la = sListenActions[laId];
            la.Dispose();
            sListenActions.Remove(laId);
        }

        /// <summary>
        /// Release all ListenActions
        /// </summary>
        public static void ReleaseAll()
        {
            sListenActions.Values.AsParallel().ForAll(la => la.Dispose());
            sListenActions.Clear();
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.EventListener.Dispose();
                    this.EventListener = null;
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
    }
}
