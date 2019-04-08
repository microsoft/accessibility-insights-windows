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
    [RuleInfo(ID = RuleId.IsKeyboardFocusableShouldBeFalse)]
    class IsKeyboardFocusableShouldBeFalse : Rule
    {
        public IsKeyboardFocusableShouldBeFalse()
        {
            this.Info.Description = Descriptions.IsKeyboardFocusableShouldBeFalse;
            this.Info.HowToFix = HowToFix.IsKeyboardFocusableShouldBeFalse;
            this.Info.Standard = A11yCriteriaId.Keyboard;
            this.Info.PropertyID = PropertyType.UIA_IsKeyboardFocusablePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return IsNotKeyboardFocusable.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Warning;
        }

        protected override Condition CreateCondition()
        {
            return ElementGroups.ExpectedNotToBeFocusable;
        }
    } // class
} // namespace
