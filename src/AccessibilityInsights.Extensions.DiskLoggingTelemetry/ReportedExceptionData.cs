// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace AccessibilityInsights.Extensions.DiskLoggingTelemetry
{
    public class ReportedExceptionData : PublishBaseData
    {
        public Exception Exception { get; set; }
    }
}
