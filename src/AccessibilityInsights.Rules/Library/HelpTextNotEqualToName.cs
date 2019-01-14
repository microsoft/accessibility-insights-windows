// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.StringProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.HelpTextNotEqualToName)]
    class HelpTextNotEqualToName : Rule
    {
        public HelpTextNotEqualToName()
        {
            this.Info.Description = Descriptions.HelpTextNotEqualToName;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            var condition = Name.IsEqualTo(HelpText);
            return condition.Matches(e) ? EvaluationCode.Warning : EvaluationCode.Pass;
        }

        protected override Condition CreateCondition()
        {
            return Name.NotNullOrEmpty & HelpText.NotNullOrEmpty;
        }
    } // class
} // namespace
