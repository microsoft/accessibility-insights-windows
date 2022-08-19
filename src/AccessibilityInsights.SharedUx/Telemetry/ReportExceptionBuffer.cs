// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Concurrent;

namespace AccessibilityInsights.SharedUx.Telemetry
{
    /// <summary>
    /// Class to handle all of the queuing of data associated with extension calls to
    /// ReportException. If any ReportException calls occur before the telemetry pipeline
    /// is fully ready to handle them, we allow the calls to be buffered, then played
    /// back when the pipeline is fully initialized.
    /// </summary>
    internal class ReportExceptionBuffer
    {
        private const int MaxBufferLength = 10;

        private readonly ConcurrentQueue<Exception> _bufferedExceptions = new ConcurrentQueue<Exception>();
        private bool _forwardExceptions;
        private readonly Action<Exception> _target;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="target">The target to receive exceptions</param>
        internal ReportExceptionBuffer(Action<Exception> target)
        {
            _target = target;
        }

        /// <summary>
        /// Enable forwarding of buffered events and flush any queued events
        /// </summary>
        internal void EnableForwarding()
        {
            _forwardExceptions = true;
            while (_bufferedExceptions.TryDequeue(out Exception exception))
            {
                _target(exception);
            }
        }

        /// <summary>
        /// Report an Exception (will be queued if forwarding is disabled)
        /// </summary>
        /// <param name="e">The Exception to buffer</param>
        internal void ReportException(Exception e)
        {
            if (e != null)
            {
                if (_forwardExceptions)
                {
                    _target(e);
                }
                else if (_bufferedExceptions.Count < MaxBufferLength)
                {
                    _bufferedExceptions.Enqueue(e);
                }
            }
        }
    }
}
