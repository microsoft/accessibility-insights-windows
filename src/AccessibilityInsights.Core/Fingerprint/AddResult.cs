// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Axe.Windows.Core.Fingerprint
{
    /// <summary>
    /// Results for fingerprint-related adding methods
    /// </summary>
    public enum AddResult
    {
        /// <summary>
        /// No change occurred because addition is not supported in this implementation
        /// </summary>
        NotSupported,

        /// <summary>
        /// No change occurred because the object being added was already present
        /// </summary>
        ItemAlreadyExists,

        /// <summary>
        /// The object being added was already present and has been updated
        /// </summary>
        ExistingItemUpdated,

        /// <summary>
        /// The object was added as a new entry in the collection
        /// </summary>
        ItemAdded,
    }
}
