// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.ControlType;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.ButtonInvokeAndExpandCollapsePatterns)]
    class ButtonInvokeAndExpandCollapsePatterns : Rule
    {
        public ButtonInvokeAndExpandCollapsePatterns()
        {
            this.Info.Description = Descriptions.ButtonInvokeAndExpandCollapsePatterns;
            this.Info.HowToFix = HowToFix.ButtonInvokeAndExpandCollapsePatterns;
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
