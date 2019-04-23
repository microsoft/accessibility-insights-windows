// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Axe.Windows.Telemetry
{
#pragma warning disable CA2237 // Mark ISerializable types with serializable
#pragma warning disable CA1032 // Implement standard exception constructors
    /// <summary>
    /// Class to wrap Exceptions that should be excluded from the Telemetry pipeline.
    /// These Exceptions are still Exceptions for control flow.
    /// </summary>
    public class ExcludedException : Exception
#pragma warning restore CA1032 // Implement standard exception constructors
#pragma warning restore CA2237 // Mark ISerializable types with serializable
    {
        /// <summary>
        /// The type of the Exception that was excluded
        /// </summary>
        public Type ExcludedType => InnerException.GetType();

        public ExcludedException(Exception innerException)
            : base (innerException.Message, innerException)
        {
        }
    }
}
