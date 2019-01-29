// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;
using static AccessibilityInsights.Rules.PropertyConditions.Relationships;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.ControlShouldSupportExpandCollapsePattern)]
    class ControlShouldSupportExpandCollapsePattern : Rule
    {
        public ControlShouldSupportExpandCollapsePattern()
        {
            this.Info.Description = Descriptions.ControlShouldSupportExpandCollapsePattern;
            this.Info.HowToFix = HowToFix.ControlShouldSupportExpandCollapsePattern;
            this.Info.Standard = A11yCriteriaId.AvailableActions;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return Patterns.ExpandCollapse.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            var complexComboBox = ComboBox & ~PlatformProperties.SimpleStyle;
            return AppBar | complexComboBox | SplitButton | (TreeItem & TreeItemChildrenExist);
        }
    } // class
} // namespace
