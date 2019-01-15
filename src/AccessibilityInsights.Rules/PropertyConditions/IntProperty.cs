// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Rules.Extensions;
using static AccessibilityInsights.Rules.PropertyConditions.General;

namespace AccessibilityInsights.Rules.PropertyConditions
{
    class IntProperty : ValueCondition<int>
    {
        public Condition Exists;
        public Condition DoesNotExist;

        public IntProperty(int propertyID)
            : base(e => e.GetPropertyValueOrDefault<int>(propertyID), string.Empty)
        {
            this.Exists = CreatePropertyExistsCondition<int>(propertyID);
            this.DoesNotExist = ~Exists;
        }
} // class
} // namespace
