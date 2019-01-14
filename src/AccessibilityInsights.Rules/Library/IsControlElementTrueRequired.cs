// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.IsControlElementTrueRequired)]
    class IsControlElementTrueRequired : Rule
    {
        public IsControlElementTrueRequired()
        {
            this.Info.ShortDescription = ShortDescriptions.IsControlElementTrueRequired;
            this.Info.Description = Descriptions.IsControlElementTrueRequired;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_IsControlElementPropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return IsControlElement.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            return BoundingRectangle.Valid & ElementGroups.IsControlElementTrueRequired;
        }
    } // class
} // namespace
