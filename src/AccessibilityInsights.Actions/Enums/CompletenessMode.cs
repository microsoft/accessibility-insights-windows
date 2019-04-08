// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Axe.Windows.Actions.Enums
{
    /// <summary>
    /// Modes when saving data
    /// </summary>
    public enum CompletenessMode
    {
        /// <summary>
        /// Full (used when saving from desktop or from CI/CD)
        /// </summary>
        Full,

        /// <summary>
        /// Privacy (used for background scans)
        /// </summary>
        Privacy,
    }
}
