// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.Resources;
using AccessibilityInsights.Rules.PropertyConditions;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;
using static AccessibilityInsights.Rules.PropertyConditions.Relationships;
using static AccessibilityInsights.Rules.PropertyConditions.StringProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.SiblingUniqueAndFocusable)]
    class SiblingUniqueAndFocusable : Rule
    {
        private static Condition EligibleChild = CreateEligibleChildCondition();

        private static Condition CreateEligibleChildCondition()
        {
            var ExcludedType = DataItem | Image | Pane | ScrollBar | Thumb | TreeItem | ListItem;

            return IsKeyboardFocusable
                & IsContentOrControlElement
                & ~ExcludedType
                & ParentExists
                & Name.NotNullOrEmpty
                & LocalizedControlType.NotNullOrEmpty
                & BoundingRectangle.Valid;
        }

        public SiblingUniqueAndFocusable()
        {
            this.Info.Description = Descriptions.SiblingUniqueAndFocusable;
            this.Info.Standard = A11yCriteriaId.NameRoleValue;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));
            if (e.Parent == null) throw new ArgumentNullException(nameof(e.Parent));

            var siblings = SiblingCount(EligibleChild
                & Name.Is(e.Name)
                & LocalizedControlType.Is(e.LocalizedControlType));
            var count = siblings.GetValue(e);
            if (count < 1) return EvaluationCode.RuleExecutionError;

            return count == 1 ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            var wpfDataItem = DataItem
                & StringProperties.Framework.Is(Core.Enums.Framework.WPF)
                & NoChild(Custom | Name.NullOrEmpty);

            return EligibleChild & NotParent(wpfDataItem);
        }
    } // class
} // namespace
