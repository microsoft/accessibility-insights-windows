// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Types;

namespace AccessibilityInsights.Rules.PropertyConditions
{
    class Patterns
    {
        public static Condition Annotation = new PatternCondition(PatternType.UIA_AnnotationPatternId);
        public static Condition CustomNavigation = new PatternCondition(PatternType.UIA_CustomNavigationPatternId);
        public static Condition Dock = new PatternCondition(PatternType.UIA_DockPatternId);
        public static Condition Drag = new PatternCondition(PatternType.UIA_DragPatternId);
        public static Condition DropTarget = new PatternCondition(PatternType.UIA_DropTargetPatternId);
        public static Condition ExpandCollapse = new PatternCondition(PatternType.UIA_ExpandCollapsePatternId);
        public static Condition Grid = new PatternCondition(PatternType.UIA_GridPatternId);
        public static Condition GridItem = new PatternCondition(PatternType.UIA_GridItemPatternId);
        public static Condition Invoke = new PatternCondition(PatternType.UIA_InvokePatternId);
        public static Condition ItemContainer = new PatternCondition(PatternType.UIA_ItemContainerPatternId);
        public static Condition LegacyIAccessible = new PatternCondition(PatternType.UIA_LegacyIAccessiblePatternId);
        public static Condition MultipleView = new PatternCondition(PatternType.UIA_MultipleViewPatternId);
        public static Condition ObjectModel = new PatternCondition(PatternType.UIA_ObjectModelPatternId);
        public static Condition RangeValue = new PatternCondition(PatternType.UIA_RangeValuePatternId);
        public static Condition Scroll = new PatternCondition(PatternType.UIA_ScrollPatternId);
        public static Condition ScrollItem = new PatternCondition(PatternType.UIA_ScrollItemPatternId);
        public static Condition Selection = new PatternCondition(PatternType.UIA_SelectionPatternId);
        public static Condition SelectionItem = new PatternCondition(PatternType.UIA_SelectionItemPatternId);
        public static Condition Selection2 = new PatternCondition(PatternType.UIA_SelectionPattern2Id);
        public static Condition Spreadsheet = new PatternCondition(PatternType.UIA_SpreadsheetPatternId);
        public static Condition SpreadsheetItem = new PatternCondition(PatternType.UIA_SpreadsheetItemPatternId);
        public static Condition Styles = new PatternCondition(PatternType.UIA_StylesPatternId);
        public static Condition SynchronizedInput = new PatternCondition(PatternType.UIA_SynchronizedInputPatternId);
        public static Condition Table = new PatternCondition(PatternType.UIA_TablePatternId);
        public static Condition TableItem = new PatternCondition(PatternType.UIA_TableItemPatternId);
        public static Condition Text = new PatternCondition(PatternType.UIA_TextPatternId);
        public static Condition TextChild = new PatternCondition(PatternType.UIA_TextChildPatternId);
        public static Condition TextEdit = new PatternCondition(PatternType.UIA_TextEditPatternId);
        public static Condition TextPattern2 = new PatternCondition(PatternType.UIA_TextPattern2Id);
        public static Condition Toggle = new PatternCondition(PatternType.UIA_TogglePatternId);
        public static Condition Transform = new PatternCondition(PatternType.UIA_TransformPatternId);
        public static Condition Transform2 = new PatternCondition(PatternType.UIA_TransformPattern2Id);
        public static Condition Value = new PatternCondition(PatternType.UIA_ValuePatternId);
        public static Condition ValueReadOnly = new PatternCondition(PatternType.UIA_ValuePatternId, ValuePatternIsReadOnly);
        public static Condition VirtualizedItem = new PatternCondition(PatternType.UIA_VirtualizedItemPatternId);
        public static Condition Window = new PatternCondition(PatternType.UIA_WindowPatternId);

        // composite pattern types
        public static Condition Actionable = CreateActionablePatternsCondition();
        public static Condition TextSelectionSupported = Condition.Create(IsTextSelectionSupported);

        private static bool ValuePatternIsReadOnly(IA11yElement e)
        {
            var pattern = e.GetPattern(PatternType.UIA_ValuePatternId);
            if (pattern == null) return false;

            return pattern.GetValue<bool>("IsReadOnly");
        }

        private static Condition CreateActionablePatternsCondition()
        {
            return CustomNavigation | Drag | Dock | ExpandCollapse
                | Invoke | RangeValue | SelectionItem | Transform
                | Transform2 | Text | Toggle ;
        }

        private static bool IsTextSelectionSupported(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            var textPattern = e.GetPattern(PatternType.UIA_TextPatternId);
            if (textPattern == null) return false;

            var supportsSelection = textPattern.GetValue<UIAutomationClient.SupportedTextSelection>("SupportedTextSelection");
                
            return supportsSelection != UIAutomationClient.SupportedTextSelection.SupportedTextSelection_None;
        }
    } // class
} // namespace
