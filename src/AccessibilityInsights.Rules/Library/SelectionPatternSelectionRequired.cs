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
    [RuleInfo(ID = RuleId.SelectionPatternSelectionRequired)]
    class SelectionPatternSelectionRequired : Rule
    {
        public SelectionPatternSelectionRequired()
        {
            this.Info.Description = Descriptions.SelectionPatternSelectionRequired;
            this.Info.HowToFix = HowToFix.SelectionPatternSelectionRequired;
            this.Info.Standard = A11yCriteriaId.AvailableActions;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return IsSelectionRequired(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        private static bool IsSelectionRequired(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            var selection = e.GetPattern(PatternType.UIA_SelectionPatternId);
            if (selection == null) return false;

            return selection.GetValue<bool>("IsSelectionRequired");
        }

        protected override Condition CreateCondition()
        {
            return Tab & Patterns.Selection & ~StringProperties.Framework.Is(Framework.Edge);
        }
    } // class
} // namespace
