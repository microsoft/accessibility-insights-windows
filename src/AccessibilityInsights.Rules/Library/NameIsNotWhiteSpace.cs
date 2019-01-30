// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.StringProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.NameNotWhiteSpace)]
    class NameIsNotWhiteSpace : Rule
    {
        public NameIsNotWhiteSpace()
        {
            this.Info.Description = Descriptions.NameNotWhiteSpace;
            this.Info.HowToFix = HowToFix.NameNotWhiteSpace;
            this.Info.Standard = A11yCriteriaId.NameRoleValue;
            this.Info.PropertyID = PropertyType.UIA_NamePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return e.Name.Trim().Length > 0 ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            return Name.NotNullOrEmpty;
        }
    } // class
} // namespace
