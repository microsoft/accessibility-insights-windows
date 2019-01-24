// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.StringProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.LocalizedControlTypeNotWhiteSpace)]
    class LocalizedControlTypeIsNotWhiteSpace : Rule
    {
        public LocalizedControlTypeIsNotWhiteSpace()
        {
            this.Info.Description = Descriptions.LocalizedControlTypeNotWhiteSpace;
            this.Info.HowToFix = HowToFix.LocalizedControlTypeNotWhiteSpace;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_LocalizedControlTypePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException("The element is null");

            return LocalizedControlType.NotWhiteSpace.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            return LocalizedControlType.NotNullOrEmpty;
        }
    } // class
} // namespace
