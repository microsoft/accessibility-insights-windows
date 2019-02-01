// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(
        ID = RuleId.NameNotNull)]
    class NameIsNotNull : Rule
    {
        public NameIsNotNull()
        {
            this.Info.Description = Descriptions.NameNotNull;
            this.Info.HowToFix = HowToFix.NameNotNull;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_NamePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) return EvaluationCode.RuleExecutionError;

            return e.Name != null ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            // Regardless if it is focusable, ProgressBar should be reported as an error

            return (IsKeyboardFocusable | ProgressBar)
                & BoundingRectangle.Valid
                & ElementGroups.NameRequired;
        }
    } // class
} // namespace
