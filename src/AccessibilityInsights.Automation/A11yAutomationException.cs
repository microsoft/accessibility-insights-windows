// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.Automation
{
    /// <summary>
    /// Wraps generic Exception with a type that we can special case in code.
    /// </summary>
    internal class A11yAutomationException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Message to report back to user--must not be trivial</param>
        /// <param name="innerException">The inner exception being wrapped</param>
        internal A11yAutomationException(string message, Exception innerException = null)
            : base(message, innerException)
        {
            if (string.IsNullOrWhiteSpace(nameof(message)))
                throw new ArgumentException("message must be non-trivial", this);
        }
    }
}
