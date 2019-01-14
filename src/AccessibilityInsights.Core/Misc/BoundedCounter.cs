// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.Core.Misc
{
    /// <summary>
    /// Class to provide an upper bound on some operation. Not thread-safe.
    /// </summary>
    public class BoundedCounter
    {
        /// <summary>
        /// The upper bound for our counter
        /// </summary>
        public int UpperBound { get; }

        /// <summary>
        /// The current count of our counter (will never exceed UpperBound)
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// The number of attempts that have occurred since the last Reset (may exceed UpperBound)
        /// </summary>
        public int Attempts { get; private set; }

        /// <summary>
        /// Returns true if the upper bound has been reached
        /// </summary>
        public bool UpperBoundExceeded => (Attempts > UpperBound);

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="upperBound">The upper bound for the counter</param>
        public BoundedCounter(int upperBound)
        {
            if (upperBound <= 0)
                throw new ArgumentException("This parameter must be positive", nameof(upperBound));

            if (upperBound == int.MaxValue)
                throw new ArgumentException("This parameter must be less than int.MaxValue", nameof(upperBound));

            UpperBound = upperBound;
            Reset();
        }

        /// <summary>
        /// Attempt to increment Count
        /// </summary>
        /// <returns>true iff we can increment without exceeding UpperBound</returns>
        public bool TryIncrement()
        {
            if (Attempts == int.MaxValue)
                return false;

            if (++Attempts > UpperBound)
                return false;

            Count = Attempts;
            return true;
        }

        /// <summary>
        /// Attempt to add a value to the count. Functional equivalent of calling
        /// TryIncrement value times.
        /// </summary>
        /// <param name="valueToAdd">The value to add. Must be non-negative</param>
        /// <returns>true iff we can add valueToAdd without exceeding UpperBound</returns>
        public bool TryAdd(int valueToAdd)
        {
            if (valueToAdd < 0)
                throw new ArgumentException("This parameter must non-negative", nameof(valueToAdd));

            // Guard against overflow
            if (int.MaxValue - valueToAdd < Attempts)
            {
                Attempts = int.MaxValue;
                Count = UpperBound;
                return false;
            }

            Attempts += valueToAdd;
            if (Attempts > UpperBound)
            {
                Count = UpperBound;
                return false;
            }

            Count = Attempts;
            return true;
        }

        /// <summary>
        /// Reset Count and Attempts, leaving UpperBound unchanged
        /// </summary>
        public void Reset()
        {
            Attempts = 0;
            Count = 0;
        }
    }
}
