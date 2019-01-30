// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.SelectionPatternSingleSelection)]
    class SelectionPatternSingleSelection : Rule
    {
        public SelectionPatternSingleSelection()
        {
            this.Info.Description = Descriptions.SelectionPatternSingleSelection;
            this.Info.HowToFix = HowToFix.SelectionPatternSingleSelection;
            this.Info.Standard = A11yCriteriaId.InfoAndRelationships;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return SelectionPattern.CanSelectMultiple.Matches(e) ? EvaluationCode.Error : EvaluationCode.Pass;
        }

        protected override Condition CreateCondition()
        {
            var controls = Spinner | Tab;
            return controls & Patterns.Selection;
        }
    } // class
} // namespace
