// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Misc;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.General;

namespace AccessibilityInsights.Rules.PropertyConditions
{
    static class BoolProperties
    {
        public static Condition IsContentElement = Condition.Create(e => e.IsContentElement, ConditionDescriptions.IsContentElement);
        public static Condition IsNotContentElement = ~IsContentElement;
        public static Condition IsContentElementExists = CreatePropertyExistsCondition<bool>(PropertyType.UIA_IsContentElementPropertyId);
        public static Condition IsContentElementDoesNotExist = ~IsContentElementExists;
        public static Condition IsControlElement = Condition.Create(e => e.IsControlElement, ConditionDescriptions.IsControlElement);
        public static Condition IsNotControlElement = ~IsControlElement;
        public static Condition IsControlElementExists = CreatePropertyExistsCondition<bool>(PropertyType.UIA_IsControlElementPropertyId);
        public static Condition IsControlElementDoesNotExist = ~IsControlElementExists;
        public static Condition IsContentOrControlElement = IsContentElement | IsControlElement;
        public static Condition IsNotContentOrControlElement = ~(IsContentElement | IsControlElement);
        public static Condition IsEnabled = Condition.Create(e => e.IsEnabled);
        public static Condition IsNotEnabled = ~IsEnabled;
        public static Condition IsKeyboardFocusable = Condition.Create(e => e.IsKeyboardFocusable);
        public static Condition IsNotKeyboardFocusable = ~IsKeyboardFocusable;
        public static Condition IsOffScreen = Condition.Create(e => e.IsOffScreen);
        public static Condition IsNotOffScreen = ~IsOffScreen;
        public static Condition IsDesktop = Condition.Create(e => e.IsRootElement());
        public static Condition IsNotDesktop = ~IsDesktop;
        public static Condition TransformPattern_CanResize = Condition.Create(e => e.TransformPatternCanResize);
    } // class
} // namespace
