// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using UIAutomationClient;

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
    }
}
