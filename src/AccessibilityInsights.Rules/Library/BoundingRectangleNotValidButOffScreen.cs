// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.Resources;
using AccessibilityInsights.Rules.PropertyConditions;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.BoundingRectangleNotValidButOffScreen)]
    class BoundingRectangleNotValidButOffScreen: Rule
    {
        public BoundingRectangleNotValidButOffScreen()
        {
            this.Info.Description = Descriptions.BoundingRectangleNotValidButOffScreen;
            this.Info.HowToFix = HowToFix.BoundingRectangleNotValidButOffScreen;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_BoundingRectanglePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return EvaluationCode.Note;
        }

        protected override Condition CreateCondition()
        {
            return IsOffScreen & BoundingRectangle.NotValid;
        }
    } // class
} // namespace
