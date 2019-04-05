// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.IntProperties;
using static Axe.Windows.Rules.PropertyConditions.Relationships;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.HeadingLevelDescendsWhenNested)]
    class HeadingLevelDescendsWhenNested : Rule
    {
        public HeadingLevelDescendsWhenNested()
        {
            this.Info.Description = Descriptions.HeadingLevelDescendsWhenNested;
            this.Info.HowToFix = HowToFix.HeadingLevelDescendsWhenNested;
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
