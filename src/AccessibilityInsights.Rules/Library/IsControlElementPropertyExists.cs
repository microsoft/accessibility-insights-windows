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
    [RuleInfo(ID = RuleId.IsControlElementPropertyExists)]
    class IsControlElementPropertyExists : Rule
    {
        public IsControlElementPropertyExists()
        {
            this.Info.Description = Descriptions.IsControlElementPropertyExists;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_IsControlElementPropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return IsControlElementExists.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            // This test should probably go away and we should always return Condition.True
            // But I am leaving it because it is the same as the old logic for the scans.
            return ElementGroups.IsControlElementTrueRequired;
        }
    } // class
} // namespace
