// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.ControlType;
using static Axe.Windows.Rules.PropertyConditions.Relationships;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.SelectionItemPatternSingleSelection)]
    class SelectionItemPatternSingleSelection : Rule
    {
        public SelectionItemPatternSingleSelection()
        {
            this.Info.Description = Descriptions.SelectionItemPatternSingleSelection;
            this.Info.HowToFix = HowToFix.SelectionItemPatternSingleSelection;
            this.Info.Standard = A11yCriteriaId.AvailableActions;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            var controlType = new ControlTypeCondition(e.ControlTypeId);
            var condition = SiblingCount(controlType & SelectionItemPattern.IsSelected) <= 1;

            return condition.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            return TabItem
                & Patterns.SelectionItem
                & SelectionItemPattern.IsSelected;
        }
    } // class
} // namespace
