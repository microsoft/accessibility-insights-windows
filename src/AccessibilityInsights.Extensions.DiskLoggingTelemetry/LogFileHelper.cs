// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;

namespace AccessibilityInsights.Extensions.DiskLoggingTelemetry
{
    internal class LogFileHelper : ILogFileHelper
    {
        private readonly string _logFileName = Path.Combine(Path.GetTempPath(), "AccessibilityInsightsLocalTelemetry.txt");

        public void ResetLogFile()
        {
            if (File.Exists(_logFileName))
            {
                File.Delete(_logFileName);
            }
        }

        public void AppendLinesToLogFile(IEnumerable<string> lines)
        {
            File.AppendAllLines(_logFileName, lines);
        }
    }
}
