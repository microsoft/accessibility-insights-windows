// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;
using static AccessibilityInsights.Rules.PropertyConditions.Relationships;
using static AccessibilityInsights.Rules.PropertyConditions.StringProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.LocalizedControlTypeNotCustom)]
    class LocalizedControlTypeIsNotCustom : Rule
    {
        public LocalizedControlTypeIsNotCustom()
        {
            this.Info.Description = Descriptions.LocalizedControlTypeNotCustom;
            this.Info.HowToFix = HowToFix.LocalizedControlTypeNotCustom;
            this.Info.Standard = A11yCriteriaId.NameRoleValue;
            this.Info.PropertyID = PropertyType.UIA_LocalizedControlTypePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException("The element is null");

            return e.LocalizedControlType != LocalizedControlTypeNames.Custom ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            var dataGridDetailsPresenter = ClassName.Is("DataGridDetailsPresenter") & Parent(DataItem);

            return Custom
                & IsKeyboardFocusable
                & LocalizedControlType.NotNullOrEmpty
                & ~dataGridDetailsPresenter;
        }
    } // class
} // namespace
