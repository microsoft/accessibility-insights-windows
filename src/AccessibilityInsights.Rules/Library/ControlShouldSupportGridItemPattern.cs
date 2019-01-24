// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;
using static AccessibilityInsights.Rules.PropertyConditions.Relationships;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.ControlShouldSupportGridItemPattern)]
    class ControlShouldSupportGridItemPattern : Rule
    {
        public ControlShouldSupportGridItemPattern()
        {
            this.Info.Description = Descriptions.ControlShouldSupportGridItemPattern;
            this.Info.HowToFix = HowToFix.ControlShouldSupportGridItemPattern;
            this.Info.Standard = A11yCriteriaId.AvailableActions;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            var condition = Patterns.GridItem | AnyChild(Patterns.GridItem);
            return condition.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            return DataItem & Parent(Patterns.Grid);
        }
    } // class
} // namespace
