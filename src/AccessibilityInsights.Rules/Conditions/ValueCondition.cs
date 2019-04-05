// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;

using static System.FormattableString;

namespace Axe.Windows.Rules
{
    /// <summary>
    /// This class enables comparisons such as ==, !=, >, and < in conditions.
    /// It takes types such as integers and strings.
    /// It is primarily used (at the moment) for the function Relationships.ChildCount.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class ValueCondition<T> : Condition where T : IComparable, IEquatable<T>
    {
        public delegate T GetterDelegate(IA11yElement e);
        private readonly string Description  = null;
        public readonly GetterDelegate GetValue = null;

        public ValueCondition(GetterDelegate getter, string description)
        {
            if (getter == null) throw new ArgumentNullException(nameof(getter));
            if (description == null) throw new ArgumentNullException(nameof(description));

            this.GetValue = getter;
            this.Description = description;
        }

        public override bool Matches(IA11yElement element)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            // this class should never be asked for its string
            throw new NotImplementedException();
        }

        /// <summary>
        /// This will always throw
        /// </summary>
        public override bool Equals(object other)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// This will always throw
        /// </summary>
        public override int GetHashCode()
        {
            throw new NotSupportedException();
        }

        public static Condition operator ==(ValueCondition<T> c, T value)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));

            if (!(value is ValueType))
                if (value == null) throw new ArgumentNullException(nameof(value));

            return new DelegateCondition(e => value.Equals(c.GetValue(e)))[Invariant($" == {value}")];
        }

        public static Condition operator !=(ValueCondition<T> c, T value)
        {
            var condition = ~(c == value);
            return condition[Invariant($" != {value}")];
        }

        public static Condition operator >(ValueCondition<T> c, T value)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));

            if (!(value is ValueType))
                if (value == null) throw new ArgumentNullException(nameof(value));

            return new DelegateCondition(e => c.GetValue(e).CompareTo(value) > 0)[Invariant($" > {value}")];
        }

        public static Condition operator <(ValueCondition<T> c, T value)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));

            if (!(value is ValueType))
                if (value == null) throw new ArgumentNullException(nameof(value));

            return new DelegateCondition(e => c.GetValue(e).CompareTo(value) < 0)[Invariant($" < {value}")];
        }

        public static Condition operator >=(ValueCondition<T> c, T value)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));

            if (!(value is ValueType))
                if (value == null) throw new ArgumentNullException(nameof(value));

            return new DelegateCondition(e => c.GetValue(e).CompareTo(value) >= 0)[Invariant($" >= {value}")];
        }

        public static Condition operator <=(ValueCondition<T> c, T value)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));

            if (!(value is ValueType))
                if (value == null) throw new ArgumentNullException(nameof(value));

            return new DelegateCondition(e => c.GetValue(e).CompareTo(value) <= 0)[Invariant($" <= {value}")];
        }
    } // class
} // namespace
