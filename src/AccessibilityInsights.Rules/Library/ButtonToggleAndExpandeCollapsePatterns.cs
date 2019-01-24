// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.ButtonToggleAndExpandCollapsePatterns)]
    class ButtonToggleAndExpandCollapsePatterns : Rule
    {
        public ButtonToggleAndExpandCollapsePatterns()
        {
            this.Info.Description = Descriptions.ButtonToggleAndExpandCollapsePatterns;
            this.Info.HowToFix = HowToFix.ButtonToggleAndExpandCollapsePatterns;
            this.Info.Standard = A11yCriteriaId.InfoAndRelationships;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            var rule = Relationships.All(Patterns.Toggle, Patterns.ExpandCollapse);

            return rule.Matches(e) ? EvaluationCode.Error : EvaluationCode.Pass;
        }

        protected override Condition CreateCondition()
        {
            return Button;
        }
    } // class
} // namespace
