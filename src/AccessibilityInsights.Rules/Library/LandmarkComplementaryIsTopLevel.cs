// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.EdgeConditions;
using static Axe.Windows.Rules.PropertyConditions.Relationships;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.LandmarkComplementaryIsTopLevel)]
    class LandmarkComplementaryIsTopLevel : Rule
    {
        public LandmarkComplementaryIsTopLevel()
        {
            this.Info.Description = Descriptions.LandmarkComplementaryIsTopLevel;
            this.Info.HowToFix = HowToFix.LandmarkComplementaryIsTopLevel;
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
            return Landmarks.Complementary;
        }
    } // class
} // namespace
