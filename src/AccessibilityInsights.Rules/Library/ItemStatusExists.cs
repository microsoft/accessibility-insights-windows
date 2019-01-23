// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;
using static AccessibilityInsights.Rules.PropertyConditions.StringProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.ItemStatusExists)]
    class ItemStatusExists : Rule
    {
        public ItemStatusExists()
        {
            this.Info.Description = Descriptions.ItemStatusExists;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_ItemStatusPropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return EvaluationCode.Open;
        }

        protected override Condition CreateCondition()
        {
            return HeaderItem & ItemStatus.Null;
        }
    } // class
} // namespace
