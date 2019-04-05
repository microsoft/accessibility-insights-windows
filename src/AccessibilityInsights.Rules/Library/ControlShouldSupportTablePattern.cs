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
    [RuleInfo(ID = RuleId.ControlShouldSupportTablePattern)]
    class ControlShouldSupportTablePattern : Rule
    {
        public ControlShouldSupportTablePattern()
        {
            this.Info.Description = Descriptions.ControlShouldSupportTablePattern;
            this.Info.HowToFix = HowToFix.ControlShouldSupportTablePattern;
            this.Info.Standard = A11yCriteriaId.AvailableActions;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            if (Patterns.Table.Matches(e))
                return EvaluationCode.Pass;

            return StringProperties.Framework.Is(Framework.Edge).Matches(e) ? EvaluationCode.Warning : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            return Calendar | Table;
        }
    } // class
} // namespace
