// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.EditSupportsOnlyValuePattern)]
    class EditSupportsOnlyValuePattern : Rule
    {
        public EditSupportsOnlyValuePattern()
        {
            this.Info.Description = Descriptions.EditSupportsOnlyValuePattern;
            this.Info.Standard = A11yCriteriaId.AvailableActions;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return EvaluationCode.Warning;
        }

        protected override Condition CreateCondition()
        {
            return Edit & Patterns.Value & ~Patterns.Text;
        }
    } // class
} // namespace
