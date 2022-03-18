// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;

namespace AccessibilityInsights.Extensions.DiskLoggingTelemetry
{
    internal class FileWrapper
    {
        private readonly Func<DateTime> _timeProvider;
        private readonly string _targetFile;

        internal FileWrapper(Func<DateTime> timeProvider)
        {
            _timeProvider = timeProvider;
            _targetFile = Path.Combine(Path.GetTempPath(), "AccessibilityInsightsLocalTelemetry.txt");
            if (File.Exists(_targetFile))
            {
                File.Delete(_targetFile);
            }
        }

        internal void InitializeFile()
        {
            WriteWithSeparatorAndTime("New Session started", null);
        }

        internal void LogThisData(string title, string data)
        {
            WriteWithSeparatorAndTime(title, data);
        }

        private void WriteWithSeparatorAndTime(string title, string data)
        {
            List<string> outputs = new List<string>();

            outputs.Add("--------------------------------------------------");
            outputs.Add($"{title} at {_timeProvider().ToUniversalTime():o}");
            if (data != null)
            {
                outputs.Add(data);
            }

            File.AppendAllLines(_targetFile, outputs);
        }
    }
}
