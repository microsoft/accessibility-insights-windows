// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.Relationships;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.BoundingRectangleOnUWPMenuBar)]
    class BoundingRectangleOnUWPMenuBar : Rule
    {
        public BoundingRectangleOnUWPMenuBar()
        {
            this.Info.Description = Descriptions.BoundingRectangleOnUWPMenuBar;
            this.Info.HowToFix = HowToFix.BoundingRectangleOnUWPMenuBar;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_BoundingRectanglePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return BoundingRectangle.NotEmpty.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Open;
        }

        protected override Condition CreateCondition()
        {
            return UWP.MenuBar & Parent(UWP.TitleBar);
        }
    } // class
} // namespace
