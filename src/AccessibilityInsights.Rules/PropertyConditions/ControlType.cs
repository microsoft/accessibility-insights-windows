// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Axe.Windows.Rules.PropertyConditions
{
    /// <summary>
    /// Contains commonly used conditions for testing against the ControlTypeId property of an IA11yElement.
    /// </summary>
    static class ControlType
    {
        public static Condition AppBar = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_AppBarControlTypeId);
        public static Condition Button = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_ButtonControlTypeId);
        public static Condition Calendar = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_CalendarControlTypeId);
        public static Condition CheckBox = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_CheckBoxControlTypeId);
        public static Condition ComboBox = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_ComboBoxControlTypeId);
        public static Condition Custom = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_CustomControlTypeId);
        public static Condition DataGrid = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_DataGridControlTypeId);
        public static Condition DataItem = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_DataItemControlTypeId);
        public static Condition Document = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_DocumentControlTypeId);
        public static Condition Edit = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_EditControlTypeId);
        public static Condition Group = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_GroupControlTypeId);
        public static Condition Header = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_HeaderControlTypeId);
        public static Condition HeaderItem = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_HeaderItemControlTypeId);
        public static Condition Hyperlink = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_HyperlinkControlTypeId);
        public static Condition Image = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_ImageControlTypeId);
        public static Condition List = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_ListControlTypeId);
        public static Condition ListItem = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_ListItemControlTypeId);
        public static Condition Menu = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_MenuControlTypeId);
        public static Condition MenuBar = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_MenuBarControlTypeId);
        public static Condition MenuItem = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_MenuItemControlTypeId);
        public static Condition Pane = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_PaneControlTypeId);
        public static Condition ProgressBar = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_ProgressBarControlTypeId);
        public static Condition RadioButton = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_RadioButtonControlTypeId);
        public static Condition ScrollBar = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_ScrollBarControlTypeId);
        public static Condition SemanticZoom = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_SemanticZoomControlTypeId);
        public static Condition Separator = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_SeparatorControlTypeId);
        public static Condition Slider = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_SliderControlTypeId);
        public static Condition Spinner = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_SpinnerControlTypeId);
        public static Condition SplitButton = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_SplitButtonControlTypeId);
        public static Condition StatusBar = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_StatusBarControlTypeId);
        public static Condition Tab = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_TabControlTypeId);
        public static Condition TabItem = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_TabItemControlTypeId);
        public static Condition Table = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_TableControlTypeId);
        public static Condition Text = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_TextControlTypeId);
        public static Condition Thumb = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_ThumbControlTypeId);
        public static Condition TitleBar = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_TitleBarControlTypeId);
        public static Condition ToolBar = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_ToolBarControlTypeId);
        public static Condition ToolTip = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_ToolTipControlTypeId);
        public static Condition Tree = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_TreeControlTypeId);
        public static Condition TreeItem = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_TreeItemControlTypeId);
        public static Condition Window = new ControlTypeCondition(Axe.Windows.Core.Types.ControlType.UIA_WindowControlTypeId);
} // class
} // namespace
