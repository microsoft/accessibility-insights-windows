// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.ControlType;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.ControlShouldNotSupportScrollPattern)]
    class ControlShouldNotSupportScrollPattern : Rule
    {
        public ControlShouldNotSupportScrollPattern()
        {
            this.Info.Description = Descriptions.ControlShouldNotSupportScrollPattern;
            this.Info.HowToFix = HowToFix.ControlShouldNotSupportScrollPattern;
            this.Info.Standard = A11yCriteriaId.AvailableActions;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return Patterns.Scroll.Matches(e) ? EvaluationCode.Error : EvaluationCode.Pass;
        }

        protected override Condition CreateCondition()
        {
            // ComboBox is handled in a separate rule
            return ScrollBar;
        }
    } // class
} // namespace
