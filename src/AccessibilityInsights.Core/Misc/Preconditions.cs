// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.Core.Misc
{
    /// <summary>
    /// Extension methods used to provide a compact way to specify preconditions
    /// </summary>
    static public class Preconditions
    {
        /// <summary>
        /// Throw ArgumentNullException if value is null
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <param name="valueName">The value name (for the exception)</param>
        public static void ArgumentIsNotNull(this object value, string valueName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(valueName, "Value can not be null");
            }
        }

        /// <summary>
        /// Throw ArgumentException if value is null
        /// </summary>
        /// <param name="value">The string to check</param>
        /// <param name="valueName">The value name (for the exception)</param>
        public static void ArgumentIsNotTrivialString(this string value, string valueName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Value can not be trivial", valueName);
            }
        }
    }
}
