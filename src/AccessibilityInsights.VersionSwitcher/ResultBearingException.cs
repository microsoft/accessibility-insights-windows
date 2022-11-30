// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights.SetupLibrary;
using System;
using System.Runtime.Serialization;

namespace AccessibilityInsights.VersionSwitcher
{
    /// <summary>
    /// The exception type thrown within VersionSwitcher. It extends Exception to
    /// specify a VersionSwitcherResult property.
    /// </summary>
    [Serializable]
    public class ResultBearingException : Exception
    {
        public ExecutionResult Result { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ResultBearingException()
        { }

        /// <summary>
        /// Constructor taking a result, a message string,
        /// and an optional inner exception to wrap
        /// </summary>
        public ResultBearingException(ExecutionResult result, string message, Exception innerException = null)
            : base(message, innerException)
        {
            if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException(nameof(message));
            Result = result;
        }

        /// <summary>
        /// Serialization constructor
        /// </summary>
        protected ResultBearingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
