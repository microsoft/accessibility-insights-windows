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
    static class ContentView
    {
        static Condition NoChildren = NoChild(IsContentElement);
        static Condition RequiresChildren = AnyChild(IsContentElement);

        public static Condition ButtonStructure = Button & NoChildren;
        public static Condition CalendarStructure = Calendar & (NoChildren | Children(ListItem))[ConditionDescriptions.Parentheses];
        public static Condition CheckBoxStructure = CheckBox & NoChildren;
        public static Condition ComboBoxStructure = ComboBox & (NoChildren | Children(ListItem))[ConditionDescriptions.Parentheses];
        public static Condition DataGridStructure = DataGrid & (NoChildren | Children(DataItem))[ConditionDescriptions.Parentheses];
        public static Condition EditStructure = Edit & NoChildren;
        public static Condition HyperlinkStructure = Hyperlink & NoChildren;
        public static Condition ListStructure = List & (NoChildren | Children(DataItem | ListItem | Group))[ConditionDescriptions.Parentheses];
        public static Condition ListItemStructure = ListItem & NoChildren;
        public static Condition MenuStructure = Menu & (RequiresChildren & Children(MenuItem));
        public static Condition ProgressBarStructure = ProgressBar & NoChildren;
        public static Condition RadioButtonStructure = RadioButton & NoChildren;
        public static Condition SliderStructure = Slider & (NoChildren | Children(ListItem))[ConditionDescriptions.Parentheses];
        public static Condition SpinnerStructure = Spinner & NoChildren;
        public static Condition SplitButtonStructure = CreateSplitButtonStructureCondition();
        public static Condition StatusBarStructure = StatusBar & (NoChildren | Children(Button | Edit | Image | ProgressBar))[ConditionDescriptions.Parentheses];
        public static Condition TabStructure = Tab & (RequiresChildren & Children(Group | TabItem));
        public static Condition TreeStructure = Tree & (NoChildren | Children(DataItem | TreeItem))[ConditionDescriptions.Parentheses];
        public static Condition TreeItemStructure = TreeItem & (NoChildren | Children(TreeItem))[ConditionDescriptions.Parentheses];

        static Condition Children(Condition childTypesCondition)
        {
            if (childTypesCondition is OrCondition
                || childTypesCondition is AndCondition)
                childTypesCondition = childTypesCondition[ConditionDescriptions.Parentheses];

            return AllChildren(~IsContentElement | childTypesCondition);
        }

        private static Condition CreateSplitButtonStructureCondition()
        {
            // from https://docs.microsoft.com/en-us/windows/desktop/WinAuto/uiauto-supportsplitbuttoncontroltype

            var P = ConditionDescriptions.Parentheses;

            var button = Button & RequiresChildren & Children(MenuItem);

            var buttons = (ChildCount(button) == 1
                | ChildCount(button) == 2)[P];

            var allChildren = Children(Button);

            return SplitButton & buttons & allChildren;
        }
    } // class
} // namespace
