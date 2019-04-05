// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.ControlType;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.EditSupportsOnlyValuePattern)]
    class EditSupportsOnlyValuePattern : Rule
    {
        public EditSupportsOnlyValuePattern()
        {
            this.Info.Description = Descriptions.EditSupportsOnlyValuePattern;
            this.Info.HowToFix = HowToFix.EditSupportsOnlyValuePattern;
            this.Info.Standard = A11yCriteriaId.AvailableActions;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return EvaluationCode.Warning;
        }

        protected override Condition CreateCondition()
        {
            return Edit & Patterns.Value & ~Patterns.Text;
        }
    } // class
} // namespace
