// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions;
using System;
using System.Collections.Concurrent;

namespace AccessibilityInsights.Desktop.Telemetry
{
    using QueueEntry = Tuple<Exception, Action<Exception>>;

    /// <summary>
    /// Class to handle all of the queuing of data associated with extension calls to
    /// ReportException. If any ReportException calls occur before the telemetry pipeline
    /// is fully ready to handle them, we allow the calls to be buffered, then played
    /// back when the pipeline is fully initialized.
    /// </summary>
    internal class ReportExceptionBuffer
    {
        private const int MaxBufferLength = 10;

        private ConcurrentQueue<QueueEntry> _bufferedExceptions = new ConcurrentQueue<QueueEntry>();
        private bool _forwardExceptions = false;

        /// <summary>
        /// Enable forwarding of buffered events and flush any queued events
        /// </summary>
        internal void EnableForwarding()
        {
            _forwardExceptions = true;
            while (_bufferedExceptions.TryDequeue(out QueueEntry entry))
            {
                entry.Item2(entry.Item1);
            }
        }

        /// <summary>
        /// Report an Exception (will be queued if forwarding is disabled)
        /// </summary>
        /// <param name="e">The Exception to buffer</param>
        /// <param name="target">The target for the exception</param>
        internal void ReportException(Exception e, Action<Exception> target)
        {
            if (e != null)
            {
                if (_forwardExceptions)
                {
                    target(e);
                }
                else if (_bufferedExceptions.Count < MaxBufferLength)
                {
                    _bufferedExceptions.Enqueue(new QueueEntry(e, target));
                }
            }
        }
    }
}
