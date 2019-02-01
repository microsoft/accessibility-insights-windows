// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;
using static AccessibilityInsights.Rules.PropertyConditions.Relationships;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.ChildrenNotAllowedInContentView)]
    class ChildrenNotAllowedInContentView : Rule
    {
        public ChildrenNotAllowedInContentView()
        {
            this.Info.Description = Descriptions.ChildrenNotAllowedInContentView;
            this.Info.HowToFix = HowToFix.ChildrenNotAllowedInContentView;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return NoChild(IsContentElement).Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            return Separator;
        }
    } // class
} // namespace
