// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.StringProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.LocalizedLandmarkTypeIsReasonableLength)]
    class LocalizedLandmarkTypeIsReasonableLength : Rule
    {
        private const int ReasonableLength = 64;

        public LocalizedLandmarkTypeIsReasonableLength()
        {
            this.Info.Description = Descriptions.LocalizedLandmarkTypeIsReasonableLength;
            this.Info.HowToFix = HowToFix.LocalizedLandmarkTypeIsReasonableLength;
            this.Info.Standard = A11yCriteriaId.InfoAndRelationships;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            var condition = LocalizedLandmarkType.Length <= ReasonableLength;
            return condition.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            return LocalizedLandmarkType.NotNullOrEmpty;
        }
    } // class
} // namespace
