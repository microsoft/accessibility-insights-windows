// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.BoolProperties;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.IsKeyboardFocusableOnEmptyContainer)]
    class IsKeyboardFocusableOnEmptyContainer : Rule
    {
        public IsKeyboardFocusableOnEmptyContainer()
        {
            this.Info.Description = Descriptions.IsKeyboardFocusableOnEmptyContainer;
            this.Info.HowToFix = HowToFix.IsKeyboardFocusableOnEmptyContainer;
            this.Info.Standard = A11yCriteriaId.Keyboard;
            this.Info.PropertyID = PropertyType.UIA_IsKeyboardFocusablePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return EvaluationCode.Open;
        }

        protected override Condition CreateCondition()
        {
            return IsNotKeyboardFocusable & IsEnabled & IsNotOffScreen & BoundingRectangle.Valid
                & ElementGroups.EmptyContainer;
        }
    } // class
} // namespace
