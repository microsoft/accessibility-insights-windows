// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Axe.Windows.Core.Fingerprint
{
    /// <summary>
    /// Abstraction for a fingerprint
    /// </summary>
    public interface IFingerprint : IComparable<IFingerprint>, IEquatable<IFingerprint>
    {
        /// <summary>
        /// The collection of Contributions contained in this fingerprint
        /// </summary>
        IReadOnlyList<FingerprintContribution> Contributions { get; }

        /// <summary>
        /// Get a human-readable string to represent this fingerprint
        /// </summary>
        /// <returns>A human-readable string for the fingerprint</returns>
        string ToString();
    }
}
