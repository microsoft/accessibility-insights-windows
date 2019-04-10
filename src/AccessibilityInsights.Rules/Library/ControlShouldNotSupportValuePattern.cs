// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.ControlType;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.ControlShouldNotSupportValuePattern)]
    class ControlShouldNotSupportValuePattern : Rule
    {
        public ControlShouldNotSupportValuePattern()
        {
            this.Info.Description = Descriptions.ControlShouldNotSupportValuePattern;
            this.Info.HowToFix = HowToFix.ControlShouldNotSupportValuePattern;
            this.Info.Standard = A11yCriteriaId.AvailableActions;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));
            
            var name = e.Name.Trim().ToLower();
            var value = e.GetPattern(PatternType.UIA_ValuePatternId).GetValue<string>("Value").Trim().ToLower();

            bool valuePatternExceedsName = value.Length > e.Name.Length || !name.Contains(value);
            return valuePatternExceedsName ? EvaluationCode.Error : EvaluationCode.Pass;
        }

        protected override Condition CreateCondition()
        {
            // Removed Calendar, even though it is specified in MSDN
            // This seems to be an error in the documentation, or at least it seems to no longer be valid
            return Text & Patterns.Value & ValuePattern.ValueProperty.NotNullOrEmpty;
        }
    } // class
} // namespace
