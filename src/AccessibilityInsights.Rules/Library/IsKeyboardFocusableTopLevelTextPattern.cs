// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;
using static AccessibilityInsights.Rules.PropertyConditions.Relationships;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.IsKeyboardFocusableTopLevelTextPattern)]
    class IsKeyboardFocusableTopLevelTextPattern : Rule
    {
        public IsKeyboardFocusableTopLevelTextPattern()
        {
            this.Info.ShortDescription = ShortDescriptions.IsKeyboardFocusableTopLevelTextPattern;
            this.Info.Description = Descriptions.IsKeyboardFocusableTopLevelTextPattern;
            this.Info.Standard = A11yCriteriaId.Keyboard;
            this.Info.PropertyID = Core.Types.PropertyType.UIA_IsKeyboardFocusablePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return IsKeyboardFocusable.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Warning;
        }

        protected override Condition CreateCondition()
        {
            return IsContentOrControlElement
                & IsEnabled & IsNotOffScreen & BoundingRectangle.Valid
                & Patterns.Text 
                & Patterns.TextSelectionSupported
                & NoAncestor(Patterns.Text)
                & ~(StringProperties.Framework.Is(Core.Enums.Framework.XAML) & ControlType.Text); // UWP case, Text control may have Text pattern without keyboard focus.
        }
    } // class
} // namespace
