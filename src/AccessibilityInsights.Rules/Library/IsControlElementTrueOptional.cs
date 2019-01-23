// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.IsControlElementTrueOptional)]
    class IsControlElementTrueOptional : Rule
    {
        public IsControlElementTrueOptional()
        {
            this.Info.Description = Descriptions.IsControlElementTrueOptional;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_IsControlElementPropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return EvaluationCode.Open;
        }

        protected override Condition CreateCondition()
        {
            return IsNotControlElement & BoundingRectangle.Valid & ElementGroups.IsControlElementTrueOptional;
        }
    } // class
} // namespace
