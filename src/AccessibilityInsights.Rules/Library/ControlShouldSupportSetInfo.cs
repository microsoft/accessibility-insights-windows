// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.Resources;
using System;
using static AccessibilityInsights.Rules.PropertyConditions.IntProperties;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;


namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.ControlShouldSupportSetInfo)]
    class ControlShouldSupportSetInfo : Rule
    {
        public ControlShouldSupportSetInfo()
        {
            this.Info.Description = Descriptions.ControlShouldSupportSetInfo;
            this.Info.HowToFix = HowToFix.ControlShouldSupportSetInfo;
            this.Info.Standard = A11yCriteriaId.InfoAndRelationships;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            if (PositionInSet.Exists.Matches(e) & SizeOfSet.Exists.Matches(e))
            {
                return EvaluationCode.Pass;
            }
            else if (PropertyConditions.StringProperties.Framework.Is(Core.Enums.Framework.WPF).Matches(e))
            {
                return EvaluationCode.Warning;
            }

            return EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            return (PropertyConditions.StringProperties.Framework.Is(Core.Enums.Framework.WPF) | PropertyConditions.StringProperties.Framework.Is(Core.Enums.Framework.XAML)) & (ListItem | TreeItem);
        }
    }
}
