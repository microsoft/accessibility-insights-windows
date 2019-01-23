// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.ButtonInvokeAndExpandCollapsePatterns)]
    class ButtonInvokeAndExpandCollapsePatterns : Rule
    {
        public ButtonInvokeAndExpandCollapsePatterns()
        {
            this.Info.Description = Descriptions.ButtonInvokeAndExpandCollapsePatterns;
            this.Info.Standard = A11yCriteriaId.InfoAndRelationships;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            var rule = Relationships.All(Patterns.Invoke, Patterns.ExpandCollapse);

            return rule.Matches(e) ? EvaluationCode.Warning : EvaluationCode.Pass;
        }

        protected override Condition CreateCondition()
        {
            return Button;
        }
    } // class
} // namespace
