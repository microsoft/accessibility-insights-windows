// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;
using static AccessibilityInsights.Rules.PropertyConditions.Relationships;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.ItemTypeRecommended)]
    class ItemTypeRecommended : Rule
    {
        public ItemTypeRecommended()
        {
            this.Info.Description = Descriptions.ItemTypeRecommended;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_ItemTypePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return EvaluationCode.Open;
        }

        protected override Condition CreateCondition()
        {
            var recommendedControls = DataItem | ListItem | TreeItem;
            return recommendedControls & NoChild(HasSameType) & AnyChild(Image);
        }
    } // class
} // namespace
