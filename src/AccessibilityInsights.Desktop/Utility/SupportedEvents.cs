// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Types;
using Axe.Windows.Desktop.Types;
using System.Collections.Generic;
using System.Linq;

namespace Axe.Windows.Desktop.Utility
{
    /// <summary>
    /// list of tuples that represent the mappings from control types to events
    ///     if the pattern ID is null, the event is required/relevant
    ///     if the pattern ID is not null, the event is only required when the pattern is present
    /// </summary>
    public static class SupportedEvents
    {
        internal static readonly IEnumerable<(int ControlId, int EventId, int? PatternId)> EventTypeMappings = new List<(int ControlId, int EventId, int? PatternId)>
        {
            (ControlType.UIA_AppBarControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_AppBarControlTypeId, EventType.UIA_StructureChangedEventId, null),
            (ControlType.UIA_AppBarControlTypeId, EventType.UIA_MenuClosedEventId, null), //relevant
            (ControlType.UIA_AppBarControlTypeId, EventType.UIA_MenuOpenedEventId, null), //relevant

            (ControlType.UIA_ButtonControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_ButtonControlTypeId, EventType.UIA_StructureChangedEventId, null),
            (ControlType.UIA_ButtonControlTypeId, EventType.UIA_Invoke_InvokedEventId, PatternType.UIA_InvokePatternId),

            (ControlType.UIA_CalendarControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_CalendarControlTypeId, EventType.UIA_LayoutInvalidatedEventId, null),
            (ControlType.UIA_CalendarControlTypeId, EventType.UIA_StructureChangedEventId, null),
            (ControlType.UIA_CalendarControlTypeId, EventType.UIA_Selection_InvalidatedEventId, null),

            (ControlType.UIA_CheckBoxControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_CheckBoxControlTypeId, EventType.UIA_StructureChangedEventId, null),

            (ControlType.UIA_ComboBoxControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_ComboBoxControlTypeId, EventType.UIA_StructureChangedEventId, null),

            (ControlType.UIA_DataGridControlTypeId, EventType.UIA_ActiveTextPositionChangedEventId, null),
            (ControlType.UIA_DataGridControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_DataGridControlTypeId, EventType.UIA_LayoutInvalidatedEventId, null),
            (ControlType.UIA_DataGridControlTypeId, EventType.UIA_StructureChangedEventId, null),
            (ControlType.UIA_DataGridControlTypeId, EventType.UIA_Selection_InvalidatedEventId, null),

            (ControlType.UIA_DataItemControlTypeId, EventType.UIA_ActiveTextPositionChangedEventId, null),
            (ControlType.UIA_DataItemControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_DataItemControlTypeId, EventType.UIA_StructureChangedEventId, null),
            (ControlType.UIA_DataItemControlTypeId, EventType.UIA_Invoke_InvokedEventId, PatternType.UIA_InvokePatternId),
            (ControlType.UIA_DataItemControlTypeId, EventType.UIA_SelectionItem_ElementAddedToSelectionEventId, PatternType.UIA_SelectionItemPatternId),
            (ControlType.UIA_DataItemControlTypeId, EventType.UIA_SelectionItem_ElementRemovedFromSelectionEventId, PatternType.UIA_SelectionItemPatternId),
            (ControlType.UIA_DataItemControlTypeId, EventType.UIA_SelectionItem_ElementSelectedEventId, PatternType.UIA_SelectionItemPatternId),

            (ControlType.UIA_DocumentControlTypeId, EventType.UIA_ActiveTextPositionChangedEventId, null),
            (ControlType.UIA_DocumentControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_DocumentControlTypeId, EventType.UIA_StructureChangedEventId, null),
            (ControlType.UIA_DocumentControlTypeId, EventType.UIA_Text_TextSelectionChangedEventId, null),
            (ControlType.UIA_DocumentControlTypeId, EventType.UIA_Text_TextChangedEventId, null),
            (ControlType.UIA_DocumentControlTypeId, EventType.UIA_Selection_InvalidatedEventId, PatternType.UIA_SelectionPatternId),

            (ControlType.UIA_EditControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_EditControlTypeId, EventType.UIA_StructureChangedEventId, null),
            (ControlType.UIA_EditControlTypeId, EventType.UIA_Text_TextSelectionChangedEventId, PatternType.UIA_TextPatternId),
            (ControlType.UIA_EditControlTypeId, EventType.UIA_Text_TextChangedEventId, PatternType.UIA_TextPatternId),

            (ControlType.UIA_GroupControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_GroupControlTypeId, EventType.UIA_StructureChangedEventId, null),

            (ControlType.UIA_HeaderControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_HeaderControlTypeId, EventType.UIA_StructureChangedEventId, null),

            (ControlType.UIA_HeaderItemControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_HeaderItemControlTypeId, EventType.UIA_StructureChangedEventId, null),
            (ControlType.UIA_HeaderItemControlTypeId, EventType.UIA_Invoke_InvokedEventId, PatternType.UIA_InvokePatternId),

            (ControlType.UIA_HyperlinkControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_HyperlinkControlTypeId, EventType.UIA_StructureChangedEventId, null),
            (ControlType.UIA_HyperlinkControlTypeId, EventType.UIA_Invoke_InvokedEventId, PatternType.UIA_InvokePatternId),

            (ControlType.UIA_ImageControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_ImageControlTypeId, EventType.UIA_StructureChangedEventId, null),

            (ControlType.UIA_ListControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_ListControlTypeId, EventType.UIA_StructureChangedEventId, null),
            (ControlType.UIA_ListControlTypeId, EventType.UIA_LayoutInvalidatedEventId, null), // "If the layout of child items can be changed, the control must support this event"
            (ControlType.UIA_ListControlTypeId, EventType.UIA_Selection_InvalidatedEventId, PatternType.UIA_SelectionPatternId),

            (ControlType.UIA_ListItemControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_ListItemControlTypeId, EventType.UIA_StructureChangedEventId, null),
            (ControlType.UIA_ListItemControlTypeId, EventType.UIA_Invoke_InvokedEventId, PatternType.UIA_InvokePatternId),
            (ControlType.UIA_ListItemControlTypeId, EventType.UIA_SelectionItem_ElementAddedToSelectionEventId, PatternType.UIA_SelectionItemPatternId),
            (ControlType.UIA_ListItemControlTypeId, EventType.UIA_SelectionItem_ElementRemovedFromSelectionEventId, PatternType.UIA_SelectionItemPatternId),
            (ControlType.UIA_ListItemControlTypeId, EventType.UIA_SelectionItem_ElementSelectedEventId, PatternType.UIA_SelectionItemPatternId),

            (ControlType.UIA_MenuControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_MenuControlTypeId, EventType.UIA_StructureChangedEventId, null),
            (ControlType.UIA_MenuControlTypeId, EventType.UIA_MenuClosedEventId, null),
            (ControlType.UIA_MenuControlTypeId, EventType.UIA_MenuOpenedEventId, null),

            (ControlType.UIA_MenuBarControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_MenuBarControlTypeId, EventType.UIA_StructureChangedEventId, null),

            (ControlType.UIA_MenuItemControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_MenuItemControlTypeId, EventType.UIA_StructureChangedEventId, null),
            (ControlType.UIA_MenuItemControlTypeId, EventType.UIA_Invoke_InvokedEventId, PatternType.UIA_InvokePatternId),
            (ControlType.UIA_MenuItemControlTypeId, EventType.UIA_SelectionItem_ElementAddedToSelectionEventId, PatternType.UIA_SelectionItemPatternId),
            (ControlType.UIA_MenuItemControlTypeId, EventType.UIA_SelectionItem_ElementRemovedFromSelectionEventId, PatternType.UIA_SelectionItemPatternId),
            (ControlType.UIA_MenuItemControlTypeId, EventType.UIA_SelectionItem_ElementSelectedEventId, PatternType.UIA_SelectionItemPatternId),

            (ControlType.UIA_PaneControlTypeId, EventType.UIA_AsyncContentLoadedEventId, null),
            (ControlType.UIA_PaneControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_PaneControlTypeId, EventType.UIA_StructureChangedEventId, null),

            (ControlType.UIA_ProgressBarControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_ProgressBarControlTypeId, EventType.UIA_StructureChangedEventId, null),

            (ControlType.UIA_RadioButtonControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_RadioButtonControlTypeId, EventType.UIA_StructureChangedEventId, null),
            (ControlType.UIA_RadioButtonControlTypeId, EventType.UIA_SelectionItem_ElementRemovedFromSelectionEventId, PatternType.UIA_SelectionItemPatternId),
            (ControlType.UIA_RadioButtonControlTypeId, EventType.UIA_SelectionItem_ElementSelectedEventId, PatternType.UIA_SelectionItemPatternId),

            (ControlType.UIA_ScrollBarControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_ScrollBarControlTypeId, EventType.UIA_StructureChangedEventId, null),

            // ControlTypes.UIA_SemanticZoomControlTypeId - no listed non-property-changed events

            (ControlType.UIA_SliderControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_SliderControlTypeId, EventType.UIA_StructureChangedEventId, null),
            (ControlType.UIA_SliderControlTypeId, EventType.UIA_Selection_InvalidatedEventId, PatternType.UIA_SelectionPatternId),

            (ControlType.UIA_SpinnerControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_SpinnerControlTypeId, EventType.UIA_StructureChangedEventId, null),

            (ControlType.UIA_SplitButtonControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_SplitButtonControlTypeId, EventType.UIA_Invoke_InvokedEventId, null),
            (ControlType.UIA_SplitButtonControlTypeId, EventType.UIA_StructureChangedEventId, null),

            (ControlType.UIA_StatusBarControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_StatusBarControlTypeId, EventType.UIA_StructureChangedEventId, null),

            (ControlType.UIA_TabControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_TabControlTypeId, EventType.UIA_StructureChangedEventId, null),

            (ControlType.UIA_TabItemControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_TabItemControlTypeId, EventType.UIA_SelectionItem_ElementRemovedFromSelectionEventId, null),
            (ControlType.UIA_TabItemControlTypeId, EventType.UIA_SelectionItem_ElementSelectedEventId, null),
            (ControlType.UIA_TabItemControlTypeId, EventType.UIA_StructureChangedEventId, null),

            (ControlType.UIA_TableControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_TableControlTypeId, EventType.UIA_StructureChangedEventId, null),

            (ControlType.UIA_TextControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_TextControlTypeId, EventType.UIA_StructureChangedEventId, null),
            (ControlType.UIA_TextControlTypeId, EventType.UIA_Text_TextChangedEventId, PatternType.UIA_TextPatternId),

            (ControlType.UIA_ThumbControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_ThumbControlTypeId, EventType.UIA_StructureChangedEventId, null),

            (ControlType.UIA_TitleBarControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_TitleBarControlTypeId, EventType.UIA_StructureChangedEventId, null),

            (ControlType.UIA_ToolBarControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_ToolBarControlTypeId, EventType.UIA_StructureChangedEventId, null),

            (ControlType.UIA_ToolTipControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_ToolTipControlTypeId, EventType.UIA_ToolTipClosedEventId, null),
            (ControlType.UIA_ToolTipControlTypeId, EventType.UIA_ToolTipOpenedEventId, null),
            (ControlType.UIA_ToolTipControlTypeId, EventType.UIA_StructureChangedEventId, null),
            (ControlType.UIA_ToolTipControlTypeId, EventType.UIA_Text_TextChangedEventId, PatternType.UIA_TextPatternId),
            (ControlType.UIA_ToolTipControlTypeId, EventType.UIA_Window_WindowClosedEventId, PatternType.UIA_WindowPatternId),
            (ControlType.UIA_ToolTipControlTypeId, EventType.UIA_Window_WindowOpenedEventId, PatternType.UIA_WindowPatternId),

            (ControlType.UIA_TreeControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_TreeControlTypeId, EventType.UIA_StructureChangedEventId, null),
            (ControlType.UIA_TreeControlTypeId, EventType.UIA_Selection_InvalidatedEventId, PatternType.UIA_SelectionPatternId),

            (ControlType.UIA_TreeItemControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_TreeItemControlTypeId, EventType.UIA_StructureChangedEventId, null),
            (ControlType.UIA_TreeItemControlTypeId, EventType.UIA_Invoke_InvokedEventId, PatternType.UIA_InvokePatternId),
            (ControlType.UIA_TreeItemControlTypeId, EventType.UIA_SelectionItem_ElementAddedToSelectionEventId, PatternType.UIA_SelectionItemPatternId),
            (ControlType.UIA_TreeItemControlTypeId, EventType.UIA_SelectionItem_ElementRemovedFromSelectionEventId, PatternType.UIA_SelectionItemPatternId),
            (ControlType.UIA_TreeItemControlTypeId, EventType.UIA_SelectionItem_ElementSelectedEventId, PatternType.UIA_SelectionItemPatternId),

            (ControlType.UIA_WindowControlTypeId, EventType.UIA_ActiveTextPositionChangedEventId, null),
            (ControlType.UIA_WindowControlTypeId, EventType.UIA_AsyncContentLoadedEventId, null),
            (ControlType.UIA_WindowControlTypeId, EventType.UIA_AutomationFocusChangedEventId, null),
            (ControlType.UIA_WindowControlTypeId, EventType.UIA_LayoutInvalidatedEventId, null),
            (ControlType.UIA_WindowControlTypeId, EventType.UIA_StructureChangedEventId, null),
            (ControlType.UIA_WindowControlTypeId, EventType.UIA_Window_WindowClosedEventId, null),
            (ControlType.UIA_WindowControlTypeId, EventType.UIA_Window_WindowOpenedEventId, null)
        };

        /// <summary>
        /// Use mapping list to get specific list for a control
        /// </summary>
        /// <param name="controlId"></param>
        /// <param name="patterns"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetEventsForControl(int controlId, List<A11yPattern> patterns)
        {
            return (from map in EventTypeMappings
                    where map.ControlId == controlId && (map.PatternId == null || patterns.Select(p=>p.Id).Contains(map.PatternId.Value))
                    select map.EventId).ToList();
        }
    }
}
