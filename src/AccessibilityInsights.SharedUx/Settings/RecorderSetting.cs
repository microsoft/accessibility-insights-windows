// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using Axe.Windows.Desktop.Types;
using System.Collections.Generic;
using System.Linq;

namespace AccessibilityInsights.SharedUx.Settings
{
    /// <summary>
    /// Event Record Configuration class
    /// it is to keep event recorder config. also it is used in WPF UI(Event Config Window)
    /// </summary>
    public class RecorderSetting : ConfigurationBase
    {
        #region public properties
#pragma warning disable CA2227 // Collection properties should be read only
        /// <summary>
        /// List of Events for recorder setting
        /// </summary>
        public IList<RecordEntitySetting> Events { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

#pragma warning disable CA2227 // Collection properties should be read only
        /// <summary>
        /// List of Properties for recorder setting
        /// </summary>
        public IList<RecordEntitySetting> Properties { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        /// <summary>
        /// Global setting listen to Focus Change event
        /// </summary>
        public bool IsListeningFocusChangedEvent { get; set; }

        /// <summary>
        /// Should event recorder ignore individual settings and listen to all events/properties
        /// </summary>
        public bool IsListeningAllEvents { get; set; }

        public ListenScope ListenScope { get; set; }
        #endregion

        public RecorderSetting()
        {
        }

        #region Static methods to get instance
        public static RecorderSetting LoadConfiguration(string path)
        {
            // Get Recorder configuration from local location. but if it is not available, get it from default location.
            RecorderSetting config = new RecorderSetting();
            config = RecorderSetting.LoadFromJSON<RecorderSetting>(path);
            if (config == null)
            {
                config = GetDefaultRecordingConfig();
                config.SerializeInJSON(path);
            }
            else
            {
                // check whether there is any new events to be added into configuration.
                var events = EventType.GetInstance();
                var ms = from e in events.GetKeyValuePairList()
                         where IsNotInList(e.Key, config.Events)
                         select e;

                if (ms.Any())
                {
                    foreach (var m in ms)
                    {
                        config.Events.Add(new RecordEntitySetting()
                        {
                            Id = m.Key,
                            Name = m.Value,
                            IsRecorded = false,
                            Type = RecordEntityType.Event,
                        });
                    }
                    config.SerializeInJSON(path);
                }
                config.IsListeningAllEvents = false;
            }

            return config;
        }

        /// <summary>
        /// check whether key exist in the given list.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="events"></param>
        /// <returns></returns>
        private static bool IsNotInList(int key, IList<RecordEntitySetting> events)
        {
            return !(from e in events
                     where e.Id == key
                     select e).Any();
        }

        /// <summary>
        /// Get the default Event Recording Configuration
        /// </summary>
        /// <returns></returns>
        public static RecorderSetting GetDefaultRecordingConfig()
        {
            RecorderSetting config = new RecorderSetting
            {
                // Set Individual Event
                Events = (from e in EventType.GetInstance().GetKeyValuePairList()
                          select new RecordEntitySetting()
                          {
                              Type = RecordEntityType.Event,
                              Id = e.Key,
                              Name = e.Value,
                              IsRecorded = false
                          }).ToList(),

                // Set properties
                Properties = (from e in PropertyType.GetInstance().GetKeyValuePairList()
                              select new RecordEntitySetting()
                              {
                                  Type = RecordEntityType.Property,
                                  Id = e.Key,
                                  Name = e.Value,
                                  IsRecorded = false
                              }).ToList(),

                // Set Global event
                IsListeningFocusChangedEvent = true,

                // Individual Event Scope
                ListenScope = ListenScope.Subtree,
            };

            return config;
        }
        #endregion
    }
}
