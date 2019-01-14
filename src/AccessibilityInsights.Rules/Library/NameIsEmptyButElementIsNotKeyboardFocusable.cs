// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;
using static AccessibilityInsights.Rules.PropertyConditions.StringProperties;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(
        ID = RuleId.NameEmptyButElementNotKeyboardFocusable)]
    class NameIsEmptyButElementIsNotKeyboardFocusable : Rule
    {
        public NameIsEmptyButElementIsNotKeyboardFocusable()
        {
            this.Info.Description = Descriptions.NameEmptyButElementNotKeyboardFocusable;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_NamePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) return EvaluationCode.RuleExecutionError;

            return Name.NotEmpty.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Open;
        }

        protected override Condition CreateCondition()
        {
            // Regardless if it is focusable, ProgressBar should be reported as an error
            // So it is handled in NameIsNotEmpty

            return IsNotKeyboardFocusable 
                & ~ProgressBar
                & Name.NotNull
                & BoundingRectangle.Valid
                & ElementGroups.NameRequired;
        }
    } // class
} // namespace
