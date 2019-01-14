// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AccessibilityInsights.Rules.PropertyConditions
{
    static class General
    {
        public static Condition CreatePropertyExistsCondition<T>(int propertyID)
        {
            return Condition.Create(e => e.TryGetPropertyValue(propertyID, out T value));
        }
    } // class
} // namespace
