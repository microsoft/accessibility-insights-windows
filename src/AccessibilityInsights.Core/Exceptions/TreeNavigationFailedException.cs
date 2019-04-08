// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Runtime.Serialization;

namespace Axe.Windows.Core.Exceptions
{
    /// <summary>
    /// Expect this exception when attempting to navigate the UIA hierarchy, or  another accessibility hierarchy, and the attempted navigation is unsuccessful.
    /// This exception is meant to indicate that the requested navigation was not possible,
    /// and any user facing error handling logic should be performed.
    /// </summary>
    [Serializable]
    public class TreeNavigationFailedException : Exception
    {
        public TreeNavigationFailedException() : base(){ }

        public TreeNavigationFailedException(string message) : base(message) { }

        public TreeNavigationFailedException(string message, Exception innerException) : base(message, innerException) { }

        protected TreeNavigationFailedException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }
}
