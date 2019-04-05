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
    [RuleInfo(ID = RuleId.ControlShouldNotSupportInvokePattern)]
    class ControlShouldNotSupportInvokePattern : Rule
    {
        public ControlShouldNotSupportInvokePattern()
        {
            this.Info.Description = Descriptions.ControlShouldNotSupportInvokePattern;
            this.Info.HowToFix = HowToFix.ControlShouldNotSupportInvokePattern;
            this.Info.Standard = A11yCriteriaId.AvailableActions;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return Patterns.Invoke.Matches(e) ? EvaluationCode.Error : EvaluationCode.Pass;
        }

        protected override Condition CreateCondition()
        {
            return AppBar | (TabItem & ~StringProperties.Framework.Is(Framework.Edge));
        }
    } // class
} // namespace
