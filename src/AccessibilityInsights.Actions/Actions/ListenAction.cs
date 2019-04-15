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
using System.Collections.Immutable;
using System.Linq;
using UIAutomationClient;

namespace Axe.Windows.Actions
{
    /// <summary>
    /// Class LstenAction
    /// to listen and record events from elements
    /// </summary>
    [InteractionLevel(UxInteractionLevel.NoUxInteraction)]
    public class ListenAction:IDisposable
    {
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
        private ListenAction(TreeScope ListenScope, ElementContext ec, HandleUIAutomationEventMessage listener)
        {
            this.Id = Guid.NewGuid();
            this.ElementContext = ec;
            this.EventListener = new EventListenerFactory(ec.Element, ListenScope);
            this.ExternalListener = listener;
        }

        /// <summary>
        /// Start recording events
        /// </summary>
        public void Start(IEnumerable<int> EventIDs, IEnumerable<int> PropertyIDs)
        {
            this.IsRunning = true;
            InitIndividualEventListeners(EventIDs);
            InitPropertyChangeListener(PropertyIDs);
        }

        /// <summary>
        /// Start Property change Event listener
        /// </summary>
        private void InitPropertyChangeListener(IEnumerable<int> propertyIds)
        {
            if (propertyIds.Any())
            {
                this.EventListener.RegisterAutomationEventListener(EventType.UIA_AutomationPropertyChangedEventId, this.onEventFired, propertyIds.ToArray());
            }
        }

        /// <summary>
        /// Start Individual Event Listener
        /// </summary>
        private void InitIndividualEventListeners(IEnumerable<int> eventIds)
        {
            foreach (var id in eventIds)
            {
                this.EventListener.RegisterAutomationEventListener(id, this.onEventFired);
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
        public static Guid CreateInstance(TreeScope ListenScope, Guid ecId, HandleUIAutomationEventMessage listener)
        {
            var ec = DataManager.GetDefaultInstance().GetElementContext(ecId);
            var la = new ListenAction(ListenScope, ec, listener);

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
