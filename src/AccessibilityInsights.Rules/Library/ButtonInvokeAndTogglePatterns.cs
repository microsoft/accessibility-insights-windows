// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.ControlType;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.ButtonInvokeAndTogglePatterns)]
    class ButtonInvokeAndTogglePatterns : Rule
    {
        public ButtonInvokeAndTogglePatterns()
        {
            this.Info.Description = Descriptions.ButtonInvokeAndTogglePatterns;
            this.Info.HowToFix = HowToFix.ButtonInvokeAndTogglePatterns;
            this.Info.Standard = A11yCriteriaId.NameRoleValue;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            var rule = Relationships.All(Patterns.Invoke, Patterns.Toggle);

            return rule.Matches(e) ? EvaluationCode.Error : EvaluationCode.Pass;
        }

        protected override Condition CreateCondition()
        {
            return Button;
        }
    } // class
} // namespace
