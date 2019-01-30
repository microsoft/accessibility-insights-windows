// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;
using static AccessibilityInsights.Rules.PropertyConditions.Relationships;
using static AccessibilityInsights.Rules.PropertyConditions.StringProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.BoundingRectangleSizeReasonable)]
    class BoundingRectangleSizeReasonable : Rule
    {
        public BoundingRectangleSizeReasonable()
        {
            this.Info.Description = Descriptions.BoundingRectangleSizeReasonable;
            this.Info.HowToFix = HowToFix.BoundingRectangleSizeReasonable;
            this.Info.Standard = A11yCriteriaId.NameRoleValue;
            this.Info.PropertyID = PropertyType.UIA_BoundingRectanglePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return BoundingRectangle.Valid.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            var ignoreableText = Text & ~IsKeyboardFocusable & Name.NullOrEmpty & ~ChildrenExist;

            return IsNotOffScreen
                & BoundingRectangle.NotNull
                & BoundingRectangle.CorrectDataFormat
                & ~ignoreableText;
        }
    } // class
} // namespace
