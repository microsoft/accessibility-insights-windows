// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Rules.Extensions;
using static Axe.Windows.Rules.PropertyConditions.General;

namespace Axe.Windows.Rules.PropertyConditions
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
