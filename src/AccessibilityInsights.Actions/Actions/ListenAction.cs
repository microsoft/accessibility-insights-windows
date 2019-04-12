// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Actions.Attributes;
using Axe.Windows.Actions.Contexts;
using Axe.Windows.Actions.Enums;
using Axe.Windows.Core.Bases;
using Axe.Windows.Desktop.Settings;
using Axe.Windows.Desktop.Types;
using Axe.Windows.Desktop.UIAutomation.EventHandlers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
        List<IA11yEventMessage> EventRecords { get; set; }
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
            this.EventRecords = new List<IA11yEventMessage>();
            this.ExternalListener = listener;
        }

        /// <summary>
        /// Start recording events
        /// </summary>
        public void Start()
        {
            this.EventRecords.Clear();
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
                lock (this)
                {
                    this.EventRecords.Add(message);
                }
                this.ExternalListener?.Invoke(message);
            }
        }

        /// <summary>
        /// Check whether there is any event recorded
        /// </summary>
        /// <returns></returns>
        public bool HasRecordedEvents()
        {
            return this.EventRecords != null && this.EventRecords.Count != 0;
        }

        #region serialization code
        /// <summary>
        /// Deserialize EventMessages from JSON file. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<EventMessage> LoadEventMessages(string path)
        {
            List<EventMessage> list = null;

            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                list = JsonConvert.DeserializeObject<List<EventMessage>>(json);
            }

            return list;
        }

        /// <summary>
        /// Save Event Messages in Json format
        /// </summary>
        /// <param name="path"></param>
        public void SaveInJson(string path)
        {
            var json = JsonConvert.SerializeObject(this.EventRecords, Formatting.Indented);

            File.WriteAllText(path, json, Encoding.UTF8);
        }
        #endregion

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
                    this.EventRecords?.ForEach(r => r.Element?.Dispose());
                    this.EventRecords.Clear();
                    this.EventRecords = null;
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
