// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.StringProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.NameWithValidBoundingRectangle)]
    class NameWithValidBoundingRectangle : Rule
    {
        public NameWithValidBoundingRectangle()
        {
            this.Info.Description = Descriptions.NameWithValidBoundingRectangle;
            this.Info.HowToFix = HowToFix.NameWithValidBoundingRectangle;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_NamePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return EvaluationCode.Warning;
        }

        protected override Condition CreateCondition()
        {
            return BoundingRectangle.NotValid & Name.NullOrEmpty & ElementGroups.NameRequired;
        }
    } // class
} // namespace
