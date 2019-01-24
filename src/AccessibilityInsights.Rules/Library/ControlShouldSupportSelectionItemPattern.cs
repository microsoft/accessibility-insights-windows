// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.ControlShouldSupportSelectionItemPattern)]
    class ControlShouldSupportSelectionItemPattern : Rule
    {
        public ControlShouldSupportSelectionItemPattern()
        {
            this.Info.Description = Descriptions.ControlShouldSupportSelectionItemPattern;
            this.Info.HowToFix = HowToFix.ControlShouldSupportSelectionItemPattern;
            this.Info.Standard = A11yCriteriaId.AvailableActions;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return Patterns.SelectionItem.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            return RadioButton | TabItem;
        }
    } // class
} // namespace
