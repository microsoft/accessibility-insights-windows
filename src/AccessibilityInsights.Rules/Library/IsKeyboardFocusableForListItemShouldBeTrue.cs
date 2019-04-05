// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.ControlType;
using static Axe.Windows.Rules.PropertyConditions.BoolProperties;
using static Axe.Windows.Rules.PropertyConditions.Relationships;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.IsKeyboardFocusableForListItemShouldBeTrue)]
    class IsKeyboardFocusableForListItemShouldBeTrue : Rule
    {
        public IsKeyboardFocusableForListItemShouldBeTrue()
        {
            this.Info.Description = Descriptions.IsKeyboardFocusableForListItemShouldBeTrue;
            this.Info.HowToFix = HowToFix.IsKeyboardFocusableForListItemShouldBeTrue;
            this.Info.Standard = A11yCriteriaId.Keyboard;
            this.Info.PropertyID = PropertyType.UIA_IsKeyboardFocusablePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return AnyChild(IsKeyboardFocusable).Matches(e) ? EvaluationCode.Warning : EvaluationCode.Pass;
        }

        protected override Condition CreateCondition()
        {
            return ListItem & IsNotKeyboardFocusable & IsNotOffScreen & IsEnabled & BoundingRectangle.Valid;
        }
    } // class
} // namespace
