// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace AccessibilityInsights.Extensions.DiskLoggingTelemetry
{
    public class PublishedEventData : PublishBaseData
    {
        public string EventName { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
        public Dictionary<string, string> EventProperties { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
