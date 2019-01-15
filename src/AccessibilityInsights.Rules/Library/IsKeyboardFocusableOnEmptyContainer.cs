// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.IsKeyboardFocusableOnEmptyContainer)]
    class IsKeyboardFocusableOnEmptyContainer : Rule
    {
        public IsKeyboardFocusableOnEmptyContainer()
        {
            this.Info.ShortDescription = ShortDescriptions.IsKeyboardFocusableOnEmptyContainer;
            this.Info.Description = Descriptions.IsKeyboardFocusableOnEmptyContainer;
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
