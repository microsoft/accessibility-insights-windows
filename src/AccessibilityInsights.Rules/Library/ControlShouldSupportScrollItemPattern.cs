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
    [RuleInfo(ID = RuleId.ControlShouldSupportScrollItemPattern)]
    class ControlShouldSupportScrollItemPattern : Rule
    {
        public ControlShouldSupportScrollItemPattern()
        {
            this.Info.Description = Descriptions.ControlShouldSupportScrollItemPattern;
            this.Info.HowToFix = HowToFix.ControlShouldSupportScrollItemPattern;
            this.Info.Standard = A11yCriteriaId.AvailableActions;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            var condition = Patterns.ScrollItem | AnyChild(Patterns.ScrollItem);
            return condition.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            return Parent(Patterns.Scroll & (ScrollPattern.HorizontallyScrollable | ScrollPattern.VerticallyScrollable))
                & (DataItem | ListItem | TreeItem);
        }
    } // class
} // namespace
