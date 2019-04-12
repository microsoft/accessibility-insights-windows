// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Types;
using Axe.Windows.Desktop.Types;
using System.Collections.Generic;
using System.Linq;
using UIAutomationClient;

namespace Axe.Windows.Desktop.Settings
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
        public List<RecordEntitySetting> Events { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

#pragma warning disable CA2227 // Collection properties should be read only
        /// <summary>
        /// List of Properties for recorder setting
        /// </summary>
        public List<RecordEntitySetting> Properties { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        /// <summary>
        /// Global setting listen to Focus Change event
        /// </summary>
        public bool IsListeningFocusChangedEvent { get; set; }

        /// <summary>
        /// Should event recorder ignore individual settings and listen to all events/properties
        /// </summary>
        public bool IsListeningAllEvents { get; set; } = false;

        public TreeScope ListenScope { get; set; }
        #endregion

        public RecorderSetting()
        {
        }

        /// <summary>
        /// Set the checked state based on id and type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="val"></param>
        public void SetChecked(int id, RecordEntityType type, bool val, string name = null)
        {
            int change = val ? 1 : -1;
            if (type == RecordEntityType.Event)
            {
                if (id == EventType.UIA_AutomationFocusChangedEventId)
                {
                    this.IsListeningFocusChangedEvent = val;
                }
                else
                {
                    this.Events.Where(e => e.Id == id).First().CheckedCount += change;                    
                }
            }
            else
            {
                if (this.Properties.Where(e => e.Id == id).Count() > 0)
                {
                    this.Properties.Where(e => e.Id == id).First().CheckedCount += change;
                }
                else
                {
                    this.Properties.Add(new RecordEntitySetting()
                    {
                        Type = RecordEntityType.Property,
                        Id = id,
                        Name = name,
                        IsCustom = true,
                        IsRecorded = false,
                        CheckedCount = 1
                    });
                }
            }
        }
    }
}
