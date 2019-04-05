// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Axe.Windows.Automation
{
    /// <summary>
    /// Extension class for CommandParameters, so that CommandParameters can remain agnostic
    /// of the data that it's handling
    /// </summary>
    static internal class CommandParametersExtensions
    {
        /// <summary>
        /// Check to see if files should be retained even when no violations are found
        /// </summary>
        /// <param name="parameters">The CommandParameters object to check</param>
        /// <returns>true iff files are always retained, even if no errors exist.
        /// Retention is our default behavior unless we're explicitly told to discard</returns>
        internal static bool RetainIfNoViolations(this CommandParameters parameters)
        {
            return !(parameters.TryGetString(CommandConstStrings.NoViolationPolicy, out string policy) &&
                policy.Equals(CommandConstStrings.Discard, StringComparison.OrdinalIgnoreCase));
        }
    }
}
