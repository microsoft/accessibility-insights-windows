// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Axe.Windows.Core.Exceptions
{
    /// <summary>
    /// Class to wrap Exceptions that should be excluded from the Telemetry pipeline
    /// </summary>
    [Serializable]
    public class TelemetryExcludedException : Exception
    {
        /// <summary>
        /// ctor <see cref="Exception"/>
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public TelemetryExcludedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// ctor <see cref="Exception"/>
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current Exception</param>
        public TelemetryExcludedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// default ctor <see cref="Exception"/>
        /// </summary>
        public TelemetryExcludedException()
        {
        }

        /// <summary>
        /// Serialization ctor <see cref="Exception"/>
        /// </summary>
        /// <param name="serializationInfo">The SerializationInfo that holds the serialized object data about the exception being thrown</param>
        /// <param name="streamingContext">The StreamingContext that contains contextual information about the source or destination</param>
        protected TelemetryExcludedException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            : base (serializationInfo, streamingContext)
        {
        }
    }
}
