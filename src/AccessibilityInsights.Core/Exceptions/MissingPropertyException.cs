// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Runtime.Serialization;

namespace AccessibilityInsights.Core.Exceptions
{
    /// <summary>
    /// MissingPropertyException
    /// thrown when there is a missing property in UIElement
    /// </summary>
    [Serializable]
    public class MissingPropertyException : Exception
    {
        public MissingPropertyException()
        {
        }

        public MissingPropertyException(string message) : base(message)
        {
        }

        public MissingPropertyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingPropertyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
