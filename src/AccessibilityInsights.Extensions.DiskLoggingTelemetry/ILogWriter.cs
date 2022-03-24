// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AccessibilityInsights.Extensions.DiskLoggingTelemetry
{
    internal interface ILogWriter
    {
        void LogThisData(string title, string data);
    }
}
