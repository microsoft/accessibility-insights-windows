// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Rules.Extensions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.General;

namespace Axe.Windows.Rules.PropertyConditions
{
    class IntProperty : ValueCondition<int>
    {
        public Condition Exists;
        public Condition DoesNotExist;

        public IntProperty(int propertyID)
            : this(propertyID, ConditionDescriptions.IntPropertyNotSet)
        { }

        /// <summary>
        /// Constructor with property description
        /// </summary>
        /// <param name="propertyID"></param>
        /// <param name="propertyDescription">
        /// String representation of an int property of an element such as "NativeWindowHandle", "HeadingLevel", etc.
        /// This may also apply to patterns, so please include the pattern name where applicable.
        /// This information may be visible to users.
        /// </param>
        public IntProperty(int propertyID, string propertyDescription)
            : base(e => e.GetPropertyValueOrDefault<int>(propertyID), propertyDescription)
        {
            this.Exists = CreatePropertyExistsCondition<int>(propertyID);
            this.DoesNotExist = ~Exists;
        }
} // class
} // namespace
