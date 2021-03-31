// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.SharedUx.Settings
{

    /// <summary>
    /// Individual Event Configuration class for recording
    /// </summary>
    public class RecordEntitySetting
    {
        public RecordEntityType Type { get; set; }
        /// <summary>
        /// Human readable Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Event Id in int
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Indicate whether it will be saved or not
        /// </summary>
        public bool IsRecorded { get; set; }

        /// <summary>
        /// Is the property custom or standard
        /// </summary>
        public bool IsCustom { get; set; }

        [NonSerialized]
        private int _checkedcount = 0;
        /// <summary>
        /// Indicate whether it will be recorded or not
        /// </summary>
        public int CheckedCount
        {
            get
            {
                return _checkedcount;
            }
            set
            {
                if (value >= 0)
                {
                    this._checkedcount = value;
                }
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public enum RecordEntityType
    {
        Event,
        Property
    }
}
