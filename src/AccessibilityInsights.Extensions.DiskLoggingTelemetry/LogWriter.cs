// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace AccessibilityInsights.Extensions.DiskLoggingTelemetry
{
    internal class LogWriter : ILogWriter
    {
        private readonly Func<DateTime> _timeProvider;
        private readonly ILogFileHelper _logFileHelper;
        private bool _haveWrittenThisSession;

        internal LogWriter(Func<DateTime> timeProvider, ILogFileHelper logFileHelper)
        {
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
            _logFileHelper = logFileHelper ?? throw new ArgumentNullException(nameof(logFileHelper));
        }

        public void LogThisData(string title, string data)
        {
            List<string> outputs = new List<string>();

            ResetIfFirstWriteInSession();

            outputs.Add("--------------------------------------------------");
            outputs.Add($"{title} at {_timeProvider().ToUniversalTime():o}");
            outputs.Add(data);

            _logFileHelper.AppendLinesToLogFile(outputs);
        }

        private void ResetIfFirstWriteInSession()
        {
            if (!_haveWrittenThisSession)
            {
                _logFileHelper.ResetLogFile();
                _haveWrittenThisSession = true;
            }
        }
    }
}
