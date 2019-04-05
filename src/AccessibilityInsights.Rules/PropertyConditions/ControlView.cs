// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.BoolProperties;
using static Axe.Windows.Rules.PropertyConditions.ControlType;
using static Axe.Windows.Rules.PropertyConditions.Relationships;

namespace Axe.Windows.Rules.PropertyConditions
{
    /// <summary>
    /// Contains conditions used to validate element hierarchy structure
    /// </summary>
    static class ControlView
    {
        static Condition NoChildren = NoChild(IsControlElement);
        static Condition RequiresChildren = AnyChild(IsControlElement);

        public static Condition ButtonStructure = Button & (NoChildren | Children(Image | Text))[ConditionDescriptions.Parentheses];
        public static Condition CalendarStructure = CreateCalendarStructureCondition();
        public static Condition ComboBoxStructure = CreateComboBoxStructureCondition();
        public static Condition CheckBoxStructure = CheckBox & NoChildren;
        public static Condition DataGridStructure = CreateDataGridStructureCondition();
        public static Condition EditStructure = Edit & NoChildren;
        public static Condition HeaderStructure = Header & (RequiresChildren & Children(HeaderItem));
        public static Condition HeaderItemStructure = HeaderItem & NoChildren;
        public static Condition HyperlinkStructure = Hyperlink & NoChildren;
        public static Condition ImageStructure = Image & NoChildren;
        public static Condition ListStructure = List & (NoChildren | Children(DataItem | ListItem | Group | ScrollBar))[ConditionDescriptions.Parentheses];
        public static Condition ListItemStructure = ListItem & (NoChildren | Children(Edit | Image | Text))[ConditionDescriptions.Parentheses];
        public static Condition MenuStructure = Menu & (RequiresChildren & Children(MenuItem));
        public static Condition ProgressBarStructure = ProgressBar & NoChildren;
        public static Condition RadioButtonStructure = RadioButton & NoChildren;
        public static Condition SemanticZoomStructure = SemanticZoom & (NoChildren | Children(List | ListItem))[ConditionDescriptions.Parentheses];
        public static Condition SeparatorStructure = Separator & NoChildren;
        public static Condition SliderStructure = CreateSliderStructureCondition();
        public static Condition SpinnerStructure = CreateSpinnerStructureCondition();
        public static Condition SplitButtonStructure = CreateSplitButtonStructureCondition();
        public static Condition StatusBarStructure = StatusBar & (NoChildren | Children(Button | Edit | Image | ProgressBar))[ConditionDescriptions.Parentheses];
        public static Condition TabStructure = Tab & (RequiresChildren & Children(Group | ScrollBar | TabItem));
        public static Condition ThumbStructure = Thumb & NoChildren;
        public static Condition ToolTipStructure = ToolTip & (NoChildren | Children(Image | Text))[ConditionDescriptions.Parentheses];
        public static Condition TreeStructure = Tree & (NoChildren | Children(DataItem | ScrollBar | TreeItem))[ConditionDescriptions.Parentheses];
        public static Condition TreeItemStructure = TreeItem & (NoChildren | Children(Button | CheckBox | Image | TreeItem))[ConditionDescriptions.Parentheses];

        static Condition Children(Condition childTypesCondition)
        {
            if (childTypesCondition is OrCondition
                || childTypesCondition is AndCondition)
                childTypesCondition = childTypesCondition[ConditionDescriptions.Parentheses];

            return AllChildren((~IsControlElement) | childTypesCondition);
        }

        private static Condition CreateCalendarStructureCondition()
        {
            var P = ConditionDescriptions.Parentheses;

            var twoButtons = ChildCount(Button) == 2;
            var noButtons = NoChild(Button);
            var buttons = noButtons | twoButtons;

            var sevenHeaderItems = ChildCount(HeaderItem) == 7;
            var noHeaderItems = NoChild(HeaderItem);
            var headerItems = noHeaderItems | sevenHeaderItems;

            var calendarHeader = Header & headerItems[P];

            var NoHeader = NoChild(Header);
            var oneHeader = ChildCount(Header) == 1;
            var headers = NoHeader | (oneHeader & AnyChild(calendarHeader));

            var structure = headers & buttons & (ChildCount(ListItem) > 0)[P];

            return Calendar / (DataGrid & structure)[P];
        }

        private static Condition CreateComboBoxStructureCondition()
        {
            // see https://docs.microsoft.com/en-us/windows/desktop/WinAuto/uiauto-supportcomboboxcontroltype

            var oneButton = ChildCount(Button) == 1;
            var optionalEdit = ChildCount(Edit) <= 1;
            var optionalList = ChildCount(List) <= 1;

            var allChildren = Children(Button | Edit | List);

            return ComboBox & oneButton & optionalEdit & optionalList & allChildren;
        }

        private static Condition CreateDataGridStructureCondition()
        {
            // from https://docs.microsoft.com/en-us/windows/desktop/WinAuto/uiauto-supportdatagridcontroltype

            var P = ConditionDescriptions.Parentheses;

            var headers = ChildCount(Header) <= 2;

            var allChildren = (NoChildren | Children(Header | DataItem))[P];

            return DataGrid & headers & allChildren;
        }

        private static Condition CreateSliderStructureCondition()
        {
            // from https://docs.microsoft.com/en-us/windows/desktop/WinAuto/uiauto-supportslidercontroltype

            var P = ConditionDescriptions.Parentheses;

            var buttons = (ChildCount(Button) == 2
                | ChildCount(Button) == 4)[P];

            var thumb = ChildCount(Thumb) == 1;

            var allChildren = Children(Button | Thumb | ListItem);

            return Slider & buttons & thumb & allChildren;
        }

        private static Condition CreateSpinnerStructureCondition()
        {
            // from https://docs.microsoft.com/en-us/windows/desktop/WinAuto/uiauto-supportspinnercontroltype

            var buttons = ChildCount(Button) == 2;
            var edit = ChildCount(Edit) <= 1;

            var allChildren = Children(Button | Edit);

            return Spinner & buttons & edit & allChildren;
        }

        private static Condition CreateSplitButtonStructureCondition()
        {
            // from https://docs.microsoft.com/en-us/windows/desktop/WinAuto/uiauto-supportsplitbuttoncontroltype

            var P = ConditionDescriptions.Parentheses;

            var menuItems = NoChildren | ChildCount(MenuItem) == 1;

            var button = Button & menuItems & Children(MenuItem);

            var buttons = (ChildCount(button) == 1
                | ChildCount(button) == 2)[P];

            var image = ChildCount(Image) <= 1;
            var text = ChildCount(Text) <= 1;

            var allChildren = Children(Button | Image | Text);

            return SplitButton & buttons & image & text & allChildren;
        }
    } // class
} // namespace
