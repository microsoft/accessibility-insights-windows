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
    [RuleInfo(ID = RuleId.NameOnOptionalType)]
    class NameOnOptionalType : Rule
    {
        public NameOnOptionalType()
        {
            this.Info.Description = Descriptions.NameOnOptionalType;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_NamePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return Name.NotNullOrEmpty.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Open;
        }

        protected override Condition CreateCondition()
        {
            return ElementGroups.NameOptional;
        }
    } // class
} // namespace
