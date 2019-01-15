// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.ControlShouldSupportTransformPattern)]
    class ControlShouldSupportTransformPattern : Rule
    {
        public ControlShouldSupportTransformPattern()
        {
            this.Info.Description = Descriptions.ControlShouldSupportTransformPattern;
            this.Info.Standard = A11yCriteriaId.AvailableActions;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return Patterns.Transform.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            // since this rule is based on the code from ScannerHeader
            // the assumption is that the test should only be true for header elements.
            // However, it seems like this should perhaps always be true.

            var controls = Header | HeaderItem;
            return controls & TransformPattern_CanResize;
        }
    } // class
} // namespace
