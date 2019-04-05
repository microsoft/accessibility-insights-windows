// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.BoolProperties;
using static Axe.Windows.Rules.PropertyConditions.ControlType;
using static Axe.Windows.Rules.PropertyConditions.Relationships;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.IsKeyboardFocusableShouldBeTrue)]
    class IsKeyboardFocusableShouldBeTrue : Rule
    {
        public IsKeyboardFocusableShouldBeTrue()
        {
            this.Info.Description = Descriptions.IsKeyboardFocusableShouldBeTrue;
            this.Info.HowToFix = HowToFix.IsKeyboardFocusableShouldBeTrue;
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
