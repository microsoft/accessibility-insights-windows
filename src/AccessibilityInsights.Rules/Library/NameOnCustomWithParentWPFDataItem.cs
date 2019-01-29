// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;
using static AccessibilityInsights.Rules.PropertyConditions.StringProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.NameOnCustomWithParentWPFDataItem)]
    class NameOnCustomWithParentWPFDataItem: Rule
    {
        public NameOnCustomWithParentWPFDataItem()
        {
            this.Info.Description = Descriptions.NameOnCustomWithParentWPFDataItem;
            this.Info.HowToFix = HowToFix.NameOnCustomWithParentWPFDataItem;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_NamePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return EvaluationCode.Note;
        }

        protected override Condition CreateCondition()
        {
            return Custom & Name.NullOrEmpty & ElementGroups.ParentWPFDataItem;
        }
    } // class
} // namespace
