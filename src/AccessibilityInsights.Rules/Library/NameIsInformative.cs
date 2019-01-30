// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Text.RegularExpressions;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.StringProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.NameIsInformative)]
    class NameIsInformative : Rule
    {
        public NameIsInformative()
        {
            this.Info.Description = Descriptions.NameIsInformative;
            this.Info.HowToFix = HowToFix.NameIsInformative;
            this.Info.Standard = A11yCriteriaId.NameRoleValue;
            this.Info.PropertyID = PropertyType.UIA_NamePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            string[] stringsToExclude =
            {
                @"^\s*Microsoft(\.(\w|\d)+)+\s*$",
                @"^\s*Windows(\.(\w|\d)+)+\s*$"
            };

            foreach (var s in stringsToExclude)
            {
                if (Regex.IsMatch(e.Name, s, RegexOptions.IgnoreCase))
                    return EvaluationCode.Error;
            }

            return EvaluationCode.Pass;
        }

        protected override Condition CreateCondition()
        {
            return Name.NotNullOrEmpty & Name.NotWhiteSpace & BoundingRectangle.Valid
                & ( ElementGroups.NameRequired | ElementGroups.NameOptional);
        }
    } // class
} // namespace
