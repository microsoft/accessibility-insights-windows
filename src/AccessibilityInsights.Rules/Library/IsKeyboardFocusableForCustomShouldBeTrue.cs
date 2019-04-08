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

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.IsKeyboardFocusableForCustomShouldBeTrue)]
    class IsKeyboardFocusableForCustomShouldBeTrue : Rule
    {
        public IsKeyboardFocusableForCustomShouldBeTrue()
        {
            this.Info.Description = Descriptions.IsKeyboardFocusableForCustomShouldBeTrue;
            this.Info.HowToFix = HowToFix.IsKeyboardFocusableForCustomShouldBeTrue;
            this.Info.Standard = A11yCriteriaId.Keyboard;
            this.Info.PropertyID = PropertyType.UIA_IsKeyboardFocusablePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            return IsKeyboardFocusable.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Warning;
        }

        protected override Condition CreateCondition()
        {
            return IsNotOffScreen & IsEnabled
                & Custom & Patterns.Actionable;
        }
    } // class
} // namespace
