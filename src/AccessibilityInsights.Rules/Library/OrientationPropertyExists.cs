// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;
using static AccessibilityInsights.Rules.PropertyConditions.IntProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.OrientationPropertyExists)]
    class OrientationPropertyExists : Rule
    {
        public OrientationPropertyExists()
        {
            this.Info.Description = Descriptions.OrientationPropertyExists;
            this.Info.HowToFix = HowToFix.OrientationPropertyExists;
            this.Info.Standard = A11yCriteriaId.InfoAndRelationships;
            this.Info.PropertyID = PropertyType.UIA_OrientationPropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return Orientation.Exists.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            return ScrollBar | Tab;
        }
    } // class
} // namespace
