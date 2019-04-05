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
    [RuleInfo(ID = RuleId.ControlShouldSupportScrollItemPattern)]
    class ControlShouldSupportScrollItemPattern : Rule
    {
        public ControlShouldSupportScrollItemPattern()
        {
            this.Info.Description = Descriptions.ControlShouldSupportScrollItemPattern;
            this.Info.HowToFix = HowToFix.ControlShouldSupportScrollItemPattern;
            this.Info.Standard = A11yCriteriaId.AvailableActions;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            var condition = Patterns.ScrollItem | AnyChild(Patterns.ScrollItem);
            return condition.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            return Parent(Patterns.Scroll & (ScrollPattern.HorizontallyScrollable | ScrollPattern.VerticallyScrollable))
                & (DataItem | ListItem | TreeItem);
        }
    } // class
} // namespace
