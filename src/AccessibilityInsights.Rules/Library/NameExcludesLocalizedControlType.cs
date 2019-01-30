// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Text.RegularExpressions;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.StringProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.NameExcludesLocalizedControlType)]
    class NameExcludesLocalizedControlType: Rule
    {
        public NameExcludesLocalizedControlType()
        {
            this.Info.Description = Descriptions.NameExcludesLocalizedControlType;
            this.Info.HowToFix = HowToFix.NameExcludesLocalizedControlType;
            this.Info.Standard = A11yCriteriaId.NameRoleValue;
            this.Info.PropertyID = PropertyType.UIA_NamePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));
            if (e.Name == null) throw new ArgumentException(nameof(e.Name));
            if (e.LocalizedControlType == null) throw new ArgumentException(nameof(e.LocalizedControlType));

            var r = new Regex($@"\b{e.LocalizedControlType}\b", RegexOptions.IgnoreCase);

            return r.IsMatch(e.Name)
                ? EvaluationCode.Error : EvaluationCode.Pass;
        }

        protected override Condition CreateCondition()
        {
            return ~ElementGroups.AllowSameNameAndControlType
                & Name.NotNullOrEmpty & Name.NotWhiteSpace
                & StringProperties.LocalizedControlType.NotNullOrEmpty & StringProperties.LocalizedControlType.NotWhiteSpace;
        }
    } // class
} // namespace
