// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.BoolProperties;
using static Axe.Windows.Rules.PropertyConditions.Relationships;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.IsKeyboardFocusableTopLevelTextPattern)]
    class IsKeyboardFocusableTopLevelTextPattern : Rule
    {
        public IsKeyboardFocusableTopLevelTextPattern()
        {
            this.Info.Description = Descriptions.IsKeyboardFocusableTopLevelTextPattern;
            this.Info.HowToFix = HowToFix.IsKeyboardFocusableTopLevelTextPattern;
            this.Info.Standard = A11yCriteriaId.Keyboard;
            this.Info.PropertyID = Axe.Windows.Core.Types.PropertyType.UIA_IsKeyboardFocusablePropertyId;
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
