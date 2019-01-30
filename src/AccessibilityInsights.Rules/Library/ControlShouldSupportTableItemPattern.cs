// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;
using static AccessibilityInsights.Rules.PropertyConditions.Relationships;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.ControlShouldSupportTableItemPattern)]
    class ControlShouldSupportTableItemPattern : Rule
    {
        public ControlShouldSupportTableItemPattern()
        {
            this.Info.Description = Descriptions.ControlShouldSupportTableItemPattern;
            this.Info.HowToFix = HowToFix.ControlShouldSupportTableItemPattern;
            this.Info.Standard = A11yCriteriaId.NameRoleValue;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            var condition = Patterns.TableItem | AnyChild(Patterns.TableItem);
            return condition.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            return DataItem & Parent(Patterns.Table);
        }
    } // class
} // namespace
