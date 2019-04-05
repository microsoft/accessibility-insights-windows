// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.BoolProperties;
using static Axe.Windows.Rules.PropertyConditions.ElementGroups;
using static Axe.Windows.Rules.PropertyConditions.Relationships;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.LandmarkNoDuplicateBanner)]
    class LandmarkNoDuplicateBanner : Rule
    {
        public LandmarkNoDuplicateBanner()
        {
            this.Info.Description = Descriptions.LandmarkNoDuplicateBanner;
            this.Info.HowToFix = HowToFix.LandmarkNoDuplicateBanner;
            this.Info.Standard = A11yCriteriaId.InfoAndRelationships;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            var edge = StringProperties.Framework.Is(Core.Enums.Framework.Edge);
            var landmark = Landmarks.Banner & (edge | IsNotOffScreen);
            var condition = DescendantCount(landmark) <= 1;
            return  condition.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            return EdgeDocument | UWP.TopLevelElement;
        }
    } // class
} // namespace
