// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.ContentViewProgressBarStructure)]
    class ContentViewProgressBarStructure: Rule
    {
        public ContentViewProgressBarStructure()
        {
            this.Info.Description = string.Format(CultureInfo.InvariantCulture, Descriptions.Structure, ContentView.ProgressBarStructure);
            this.Info.HowToFix = string.Format(CultureInfo.InvariantCulture, HowToFix.Structure, ContentView.ProgressBarStructure);
            this.Info.Standard = A11yCriteriaId.InfoAndRelationships;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return ContentView.ProgressBarStructure.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Note;
        }

        protected override Condition CreateCondition()
        {
            return ProgressBar;
        }
    } // class
} // namespace
