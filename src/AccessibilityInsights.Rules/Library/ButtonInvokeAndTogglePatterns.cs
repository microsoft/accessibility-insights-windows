// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.ButtonInvokeAndTogglePatterns)]
    class ButtonInvokeAndTogglePatterns : Rule
    {
        public ButtonInvokeAndTogglePatterns()
        {
            this.Info.Description = Descriptions.ButtonInvokeAndTogglePatterns;
            this.Info.HowToFix = HowToFix.ButtonInvokeAndTogglePatterns;
            this.Info.Standard = A11yCriteriaId.InfoAndRelationships;
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
