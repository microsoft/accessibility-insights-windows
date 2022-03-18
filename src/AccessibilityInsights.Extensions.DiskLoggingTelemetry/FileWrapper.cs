// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
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
            WriteWithTimeAndSeparator("New Session started", null);
        }

        internal void LogThisData(string title, string data)
        {
            WriteWithTimeAndSeparator(title, data);
        }

        private void WriteWithTimeAndSeparator(string title, string data)
        {
            string[] outputs = new string[3];

            outputs[0] = "--------------------------------------------------";
            outputs[1] = $"{title} at {_timeProvider().ToUniversalTime().ToString("o")}";
            outputs[2] = data;

            File.AppendAllLines(_targetFile, outputs);
        }
    }
}
