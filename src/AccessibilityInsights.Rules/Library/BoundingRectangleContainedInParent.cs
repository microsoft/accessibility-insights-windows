// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.Extensions;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;
using static AccessibilityInsights.Rules.PropertyConditions.Relationships;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.BoundingRectangleContainedInParent)]
    class BoundingRectangleContainedInParent : Rule
    {
        const int Margin = BoundingRectangle.OverlapMargin;

        public BoundingRectangleContainedInParent()
        {
            this.Info.Description = Descriptions.BoundingRectangleContainedInParent;
            this.Info.HowToFix = HowToFix.BoundingRectangleContainedInParent;
            this.Info.Standard = A11yCriteriaId.NameRoleValue;
            this.Info.PropertyID = PropertyType.UIA_BoundingRectanglePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            if (!IsBoundingRectangleContained(e.Parent, e))
            {
                // if the element is not contained in the parent element, go further 
                var container = e.FindContainerElement();

                return IsBoundingRectangleContained(container, e) ? EvaluationCode.Pass : EvaluationCode.Error;
            }

            return EvaluationCode.Pass;
        }

        private static bool IsBoundingRectangleContained(IA11yElement container, IA11yElement containee)
        {
            if (container == null) throw new Exception("Expected a valid container element.");
            if (containee == null) throw new Exception("Expected a valid containee element.");

            if (ScrollPattern.NotHorizontallyScrollable.Matches(container))
            {
                if (container.BoundingRectangle.Left - Margin > containee.BoundingRectangle.Left)
                    return false;

                if (container.BoundingRectangle.Right + Margin < containee.BoundingRectangle.Right)
                    return false;
            }

            if (ScrollPattern.NotVerticallyScrollable.Matches(container))
            {
                if (container.BoundingRectangle.Top - Margin > containee.BoundingRectangle.Top)
                    return false;

                if (container.BoundingRectangle.Bottom + Margin < containee.BoundingRectangle.Bottom)
                    return false;
            }

            return true;
        }

        protected override Condition CreateCondition()
        {
            // Windows can be any size, regardless of their parents

            return ~Window
                & IsNotOffScreen
                & BoundingRectangle.Valid
                & ParentExists
                & Parent(IsNotDesktop)
                & Parent(BoundingRectangle.Valid)
                & ~BoundingRectangle.CompletelyObscuresContainer;
        }
    } // class
} // namespace
