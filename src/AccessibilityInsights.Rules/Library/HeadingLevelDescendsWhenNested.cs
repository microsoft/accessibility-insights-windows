// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.IntProperties;
using static AccessibilityInsights.Rules.PropertyConditions.Relationships;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.HeadingLevelDescendsWhenNested)]
    class HeadingLevelDescendsWhenNested : Rule
    {
        public HeadingLevelDescendsWhenNested()
        {
            this.Info.Description = Descriptions.HeadingLevelDescendsWhenNested;
            this.Info.Standard = A11yCriteriaId.InfoAndRelationships;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            var condition = HeadingLevel > e.HeadingLevel;
            return AnyAncestor(condition).Matches(e) ? EvaluationCode.Error : EvaluationCode.Pass;
        }

        protected override Condition CreateCondition()
        {
            return (HeadingLevel >= HeadingLevelType.HeadingLevel1) & (HeadingLevel <= HeadingLevelType.HeadingLevel9);
        }
    } // class
} // namespace
