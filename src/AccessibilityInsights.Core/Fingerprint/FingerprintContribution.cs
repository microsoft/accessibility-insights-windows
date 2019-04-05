// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Misc;
using System;

namespace Axe.Windows.Core.Fingerprint
{
    /// <summary>
    /// Contains a single contribution to a fingerprint
    /// </summary>
#pragma warning disable CA1036 // Override methods on comparable types
    public class FingerprintContribution : IComparable<FingerprintContribution>, IEquatable<FingerprintContribution>
#pragma warning restore CA1036 // Override methods on comparable types
    {
        // Use Tuple here to simplify hashing, and becuase we'll probably have additional fields in the future
        private readonly ValueTuple<string, string> _tuple;

        /// <summary>
        /// The key associated with this contribution. Will always be non-trivial
        /// </summary>
        public string Key { get => _tuple.Item1; }

        /// <summary>
        /// The value associate with this contribution. Will always be non-trival
        /// </summary>
        public string Value { get => _tuple.Item2; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="key">The non-trivial key to identify this contribution</param>
        /// <param name="value">The non-trivial value for this contribution</param>
        public FingerprintContribution(string key, string value)
        {
            key.ArgumentIsNotTrivialString(nameof(key));
            value.ArgumentIsNotTrivialString(nameof(value));

            _tuple = new ValueTuple<string, string>(key, value);
        }

        /// <summary>
        /// Implementation of IComparable
        /// </summary>
        /// <param name="other">The other object to consider for comparison</param>
        /// <returns> less than 0: this is before other
        ///           0: this is in same position as other
        ///           greater than 0: this is after other</returns>
        public int CompareTo(FingerprintContribution other)
        {
            other.ArgumentIsNotNull(nameof(other));

            if (Key == other.Key)
                return string.CompareOrdinal(Value, other.Value);

            return string.CompareOrdinal(Key, other.Key);
        }

        /// <summary>
        /// Implementation of IEquatable
        /// </summary>
        /// <param name="other">The other object to consider for equality</param>
        /// <returns>true if the objects are equivalent</returns>
        public override bool Equals(object other)
        {
            FingerprintContribution typedOther = other as FingerprintContribution;

            if (typedOther == null)
                return false;

            return Equals(typedOther);
        }

        /// <summary>
        /// Implementation of IEquatable
        /// </summary>
        /// <param name="other">The other object to consider for equality</param>
        /// <returns>true if the objects are equivalent</returns>
        public bool Equals(FingerprintContribution other)
        {
            if (other == null)
                return false;

            return Key == other.Key && Value == other.Value;
        }

        /// <summary>
        /// Get a hash code for the contribution
        /// </summary>
        /// <returns>A hash code</returns>
        public override int GetHashCode()
        {
            return _tuple.GetHashCode();
        }
    }
}
