// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.ControlType;
using static Axe.Windows.Rules.PropertyConditions.Relationships;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.LandmarkOneMain)]
    class LandmarkOneMain : Rule
    {
        public LandmarkOneMain()
        {
            this.Info.Description = Descriptions.LandmarkOneMain;
            this.Info.HowToFix = HowToFix.LandmarkOneMain;
            this.Info.Standard = A11yCriteriaId.InfoAndRelationships;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            var condition = DescendantCount(Landmarks.Main) == 1;
            return condition.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Warning;
        }

        protected override Condition CreateCondition()
        {
            return Pane & StringProperties.Framework.Is(Core.Enums.Framework.Edge) & NotParent(StringProperties.Framework.Is(Core.Enums.Framework.Edge));
        }
    } // class
} // namespace
