// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using static AccessibilityInsights.Rules.PropertyConditions.General;

namespace AccessibilityInsights.Rules.PropertyConditions
{
    class EnumProperty<T> where T : IConvertible
    {
        private readonly int PropertyID;
        public readonly Condition Exists;
        public readonly Condition DoesNotExist;

        public EnumProperty(int propertyID)
        {
            if (!typeof(T).IsEnum) throw new InvalidOperationException($"Expected {nameof(T)} to be an enumeration type");

            this.PropertyID = propertyID;
            this.Exists = CreatePropertyExistsCondition<int>(propertyID);
            this.DoesNotExist = ~Exists;
        }

        private T GetPropertyValue(IA11yElement e)
        {
            /*
             * In this function we return default(T) in the case of an invalid value.
             * Because T will always be an enum, default(T) will always be int 0.
             * In UIA, there are no enum values, only integer values.
             * These values will never be 0.
             * Therefore, returning 0 will never cause an EnumProperty to match accidentally.
             * Thus being correct, without throwing an exception or returning a valid default value
             * on a rule which may not care about the given enum value at all.
             */

            if (!e.TryGetPropertyValue(this.PropertyID, out int i)) return default(T);
            if (!Enum.IsDefined(typeof(T), i)) return default(T);

            return (T)Enum.ToObject(typeof(T), i);
        }

        public static Condition operator ==(EnumProperty<T> p, T value)
        {
            return Condition.Create(e => p.GetPropertyValue(e).Equals(value));
        }

        public static Condition operator !=(EnumProperty<T> p, T value)
        {
            return ~(p == value);
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
    } // class
} // namespace
