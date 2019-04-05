// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.ControlType;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.ButtonToggleAndExpandCollapsePatterns)]
    class ButtonToggleAndExpandCollapsePatterns : Rule
    {
        public ButtonToggleAndExpandCollapsePatterns()
        {
            this.Info.Description = Descriptions.ButtonToggleAndExpandCollapsePatterns;
            this.Info.HowToFix = HowToFix.ButtonToggleAndExpandCollapsePatterns;
            this.Info.Standard = A11yCriteriaId.NameRoleValue;
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
