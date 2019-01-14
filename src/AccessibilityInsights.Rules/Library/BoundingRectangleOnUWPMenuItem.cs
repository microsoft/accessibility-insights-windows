// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;
using static AccessibilityInsights.Rules.PropertyConditions.Relationships;
using static AccessibilityInsights.Rules.PropertyConditions.StringProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.BoundingRectangleOnUWPMenuItem)]
    class BoundingRectangleOnUWPMenuItem : Rule
    {
        public BoundingRectangleOnUWPMenuItem()
        {
            this.Info.Description = Descriptions.BoundingRectangleOnUWPMenuItem;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_BoundingRectanglePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return BoundingRectangle.NotEmpty.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Open;
        }

        protected override Condition CreateCondition()
        {
            var grandparent = Ancestor(2, UWP.TitleBar);
            var parent = Parent(UWP.MenuBar & BoundingRectangle.Null);
            var self = MenuItem & Name.Is("System");
                return self & parent & grandparent;
        }
    } // class
} // namespace
