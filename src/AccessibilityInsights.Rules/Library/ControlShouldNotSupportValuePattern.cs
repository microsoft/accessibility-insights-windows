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
    [RuleInfo(ID = RuleId.ControlShouldNotSupportValuePattern)]
    class ControlShouldNotSupportValuePattern : Rule
    {
        public ControlShouldNotSupportValuePattern()
        {
            this.Info.Description = Descriptions.ControlShouldNotSupportValuePattern;
            this.Info.Standard = A11yCriteriaId.AvailableActions;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return Patterns.Value.Matches(e) ? EvaluationCode.Warning : EvaluationCode.Pass;
        }

        protected override Condition CreateCondition()
        {
            // Removed Calendar, even though it is specified in MSDN
            // This seems to be an error in the documentation, or at least it seems to no longer be valid
            return Text;
        }
    } // class
} // namespace
