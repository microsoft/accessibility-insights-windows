// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.Resources;
using AccessibilityInsights.Rules.PropertyConditions;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.ProgressBarRangeValue)]
    class ProgressBarRangeValue : Rule
    {
        public ProgressBarRangeValue()
        {
            this.Info.Description = Descriptions.ProgressBarRangeValue;
            this.Info.HowToFix = HowToFix.ProgressBarRangeValue;
            this.Info.Standard = A11yCriteriaId.NameRoleValue;
            this.Info.PropertyID = PropertyType.UIA_IsRangeValuePatternAvailablePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return AreMinMaxValuesCorrect(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        private static bool AreMinMaxValuesCorrect(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            var rangeValue = e.GetPattern(PatternType.UIA_RangeValuePatternId);
            if (rangeValue == null) throw new Exception($"Expected {nameof(rangeValue)} not to be null");

            return PropertyValueMatches(rangeValue, "Minimum", 0.0)
                && PropertyValueMatches(rangeValue, "Maximum", 100.0)
                && PropertyValueMatches(rangeValue, "IsReadOnly", true);
        }

        private static bool PropertyValueMatches<T>(IA11yPattern pattern, string name, T value) where T : IEquatable<T>
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            var p = pattern.GetValue<T>(name);
            var equatable = p as IEquatable<T>;
            if (equatable == null) return false;

            return equatable.Equals(value);
        }

        protected override Condition CreateCondition()
        {
            return ProgressBar & Patterns.RangeValue;
        }
    } // class
} // namespace
