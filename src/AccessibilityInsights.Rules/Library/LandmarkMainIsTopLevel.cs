// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.EdgeConditions;
using static AccessibilityInsights.Rules.PropertyConditions.Relationships;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.LandmarkMainIsTopLevel)]
    class LandmarkMainIsTopLevel : Rule
    {
        public LandmarkMainIsTopLevel()
        {
            this.Info.Description = Descriptions.LandmarkMainIsTopLevel;
            this.Info.Standard = A11yCriteriaId.InfoAndRelationships;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            var stopCondition = InsideEdge.Matches(e) ? NotInsideEdge : Condition.False;

            return AnyAncestor(Landmarks.Any, stopCondition).Matches(e) ? EvaluationCode.Error : EvaluationCode.Pass;
        }

        protected override Condition CreateCondition()
        {
            return Landmarks.Main;
        }
    } // class
} // namespace
