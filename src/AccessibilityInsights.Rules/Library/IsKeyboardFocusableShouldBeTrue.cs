// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;
using static AccessibilityInsights.Rules.PropertyConditions.Relationships;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.IsKeyboardFocusableShouldBeTrue)]
    class IsKeyboardFocusableShouldBeTrue : Rule
    {
        public IsKeyboardFocusableShouldBeTrue()
        {
            this.Info.Description = Descriptions.IsKeyboardFocusableShouldBeTrue;
            this.Info.Standard = A11yCriteriaId.Keyboard;
            this.Info.PropertyID = PropertyType.UIA_IsKeyboardFocusablePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return IsKeyboardFocusable.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Warning;
        }

        protected override Condition CreateCondition()
        {
            // List items are a special case. They are handled by IsKeyboardFocusableForListItemShouldBeTrue
            return IsEnabled
                & IsNotOffScreen
                & ~(ListItem & AnyChild(IsKeyboardFocusable))
                & ElementGroups.ExpectedToBeFocusable;
        }
    } // class
} // namespace
