// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace AccessibilityInsights.Core.Bases
{
    public interface IA11yEventMessage
    {
        int EventId { get; set; }

        /// <summary>
        /// Time stamp with millisecond accuracy
        /// </summary>
        string TimeStamp { get; set; }

        List<KeyValuePair<string, dynamic>> Properties { get; }

        A11yElement Element { get; set; }
    }
}
