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
    [RuleInfo(ID = RuleId.IsContentElementFalseOptional)]
    class IsContentElementFalseOptional : Rule
    {
        public IsContentElementFalseOptional()
        {

            this.Info.Description = Descriptions.IsContentElementFalseOptional;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_IsContentElementPropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return EvaluationCode.Open;
        }

        protected override Condition CreateCondition()
        {
            var controls = AppBar | Header | HeaderItem | MenuBar
                | ScrollBar | Separator | Thumb | TitleBar;

            return IsContentElement & controls & BoundingRectangle.Valid;
        }
    } // class
} // namespace
