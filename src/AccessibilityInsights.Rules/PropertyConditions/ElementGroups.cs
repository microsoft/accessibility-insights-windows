// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Misc;
using AccessibilityInsights.Core.Types;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;
using static AccessibilityInsights.Rules.PropertyConditions.Relationships;
using static AccessibilityInsights.Rules.PropertyConditions.StringProperties;

namespace AccessibilityInsights.Rules.PropertyConditions
{
    /// <summary>
    /// A collection of conditions representing elements grouped for miscellaneous  rules.
    /// These are conditions which tend to be reused by multiple rules.
    /// </summary>
    static class ElementGroups
    {
        // the following occurs for xaml expand/collapse controls
        private static Condition FocusableGroup = Group & IsKeyboardFocusable & (StringProperties.Framework.Is(Core.Enums.Framework.WPF) | StringProperties.Framework.Is(Core.Enums.Framework.XAML));

        public static Condition MinMaxCloseButton = CreateMinMaxCloseButtonCondition();
        public static Condition FocusableButton = CreateFocusableButtonCondition();
        private static Condition UnfocusableControlsBasedOnExplorer = CreateUnfocusableControlsBasedOnExplorerCondition();
        private static Condition UnfocusableControlsBasedOnOffice = CreateUnfocusableControlsBasedOnOfficeCondition();
        public static Condition ExpectedNotToBeFocusable = CreateExpectedNotToBeFocusableCondition();
        public static Condition Container = CreateContainerCondition();
        public static Condition EmptyContainer = CreateEmptyContainerCondition();
        public static Condition ExpectedToBeFocusable = CreateExpectedToBeFocusableCondition();
        public static Condition ParentWPFDataItem = CreateParentWPFDataItemCondition();
        public static Condition WPFScrollBarPageUpOrPageDownButtons = CreateWPFScrollBarPageUpOrPageDownButtons();
        public static Condition NameRequired = CreateNameRequiredCondition();
        public static Condition NameOptional = CreateNameOptionalCondition();
        public static Condition IsControlElementTrueRequired = CreateIsControlRequiredCondition();
        public static Condition IsControlElementTrueOptional = CreateIsControlOptionalCondition();
        public static Condition EdgeDocument = Pane & StringProperties.Framework.Is(Core.Enums.Framework.Edge) & NotParent(StringProperties.Framework.Is(Core.Enums.Framework.Edge));

        public static Condition AllowSameNameAndControlType = CreateAllowSameNameAndControlTypeCondition();

        private static Condition CreateMinMaxCloseButtonCondition()
        {
            var close = AutomationID.Is("Close");
            var minimize = AutomationID.Is("Minimize");
            var maximize = AutomationID.Is("Maximize");
            return Button & (close | minimize | maximize);
        }

        private static Condition CreateFocusableButtonCondition()
        {
            var parents = NotParent(ComboBox | ScrollBar | Slider | Spinner | SplitButton | TitleBar);
            return Button & parents & ~MinMaxCloseButton;
        }

        private static Condition CreateUnfocusableControlsBasedOnExplorerCondition()
        {
            var IsDirectUIFramework = StringProperties.Framework.Is("DirectUI");

            // Based on Win10 Explorer behavior, these exclusions are made.
            return (Button & ClassName.Is("UIExpandoButton") & IsDirectUIFramework)
                | (SplitButton & ~IsKeyboardFocusable & SecondChild & Parent(Group) & (SiblingCount() == 2) & SiblingsOfSameType)
                | (Edit & Patterns.ValueReadOnly)
                | (Edit & ClassName.Is("UIProperty") & IsDirectUIFramework)
                | (Button & Parent(SplitButton) & IsDirectUIFramework & AutomationID.Is("Dropdown"));
        }

            private static Condition CreateUnfocusableControlsBasedOnOfficeCondition()
        {
            // the following menu item never gets focus because its child, an edit field, always gets focus
            var quickHelpMenuItem = MenuItem & AutomationID.Is("TellMeControlAnchor");

            var buttons = Button
                & (ClassName.Is("NetUIAppFrameHelper") // min, max, close buttons for documents/spreadsheets
                | ClassName.Is("NetUIFolderBarRoot")
                | ClassName.Is("NetUIStickyButton")); // pin buttons on Word home screen.

            return buttons | quickHelpMenuItem;
        }

        private static Condition CreateExpectedNotToBeFocusableCondition()
        {
            /// originally, menu was the part of this list. 
            /// however based on feedback from Guru(Trust tester) and MSDN, menu is now counted as keyboard focusable element. 
            return (Button & ~FocusableButton) | Header | ScrollBar | SemanticZoom;
        }

        private static Condition CreateContainerCondition()
        {
            return DataGrid | List | Table | Tree;
        }

        private static Condition CreateEmptyContainerCondition()
        {
            return (DataGrid & NoDescendant(DataItem))
                | (List & NoDescendant(ListItem))
                | (Table & NoDescendant(DataItem))
                | (Tree & NoDescendant(TreeItem));
        }

        private static Condition CreateExpectedToBeFocusableCondition()
        {
            // the following types were excluded by the old scans code.
            // I am putting them in here for fidelity, not because they are necessarily correct.
            // the ignore list used to include SpinButton, which was removed below because that seems like an error.
            // The Tree type was also ignored. It was moved to the EmptyContainer condition.
            // exclude custom from this scan since Custom control type behavior is a matter of App implementation. 
            // titlebar and image are added here based on the V1 rule since the two are causing extra erros on Office apps.
            var ignoreTypes = AppBar | Custom | DataItem | Group | Image | Header | Pane | Separator | StatusBar | TitleBar | Thumb | Text | ToolBar | ToolTip | Window;

            ignoreTypes |= (ListItem & Parent(ComboBox)) | Container;
            ignoreTypes |= ExpectedNotToBeFocusable;

            return IsContentOrControlElement 
                 & BoundingRectangle.Valid 
                 & ~ignoreTypes
                 & ~UnfocusableControlsBasedOnExplorer
                 & ~UnfocusableControlsBasedOnOffice;
        }

        private static Condition CreateNameRequiredCondition()
        {
            var allowedCustom = Custom - ParentWPFDataItem;
            var trueWhenElementHasSiblingsOfSameType = (Header | StatusBar) & SiblingsOfSameType;
            var allowedImage = Image & NotParent(Button | ListItem | MenuItem | TreeItem);
            var allowedText = Text & Condition.Create(HasAllowedPlatformPropertiesForText);

            return ElementGroups.FocusableButton | Calendar | CheckBox | ComboBox
                | allowedCustom | DataGrid | DataItem | Document
                | Edit | FocusableGroup | HeaderItem | Hyperlink
                | allowedImage | List | ListItem | Menu
                | MenuBar | MenuItem | ProgressBar
                | RadioButton | SemanticZoom | Slider | Spinner
                | SplitButton | TabItem
                | Table | allowedText | ToolBar 
                | ToolTip | Tree | TreeItem | Window
                | trueWhenElementHasSiblingsOfSameType;
        }

        private static Condition CreateNameOptionalCondition()
        {
            return (Group & ~FocusableGroup) | Pane;
        }

        private static Condition CreateParentWPFDataItemCondition()
        {
            return Condition.Create(IsParentWPFDataItem);
        }

        private static Condition CreateWPFScrollBarPageUpOrPageDownButtons()
        {
            return Button
                & Parent(ScrollBar)
                & Framework.Is(AccessibilityInsights.Core.Enums.Framework.WPF)
                & (AutomationID.Is("PageUp")
                | AutomationID.Is("PageDown"));
        }

        private static bool IsParentWPFDataItem(IA11yElement e)
        {
            return e.GetUIFramework() == Core.Enums.Framework.WPF
                && e.Parent != null
                && e.Parent.ControlTypeId == Core.Types.ControlType.UIA_DataItemControlTypeId;
        }

        private static bool HasAllowedPlatformPropertiesForText(IA11yElement e)
        {
            return !IsPlatformWinForms(e)
                || !HasStaticEdgeExtendedStyle(e);
        }

        private static bool HasStaticEdgeExtendedStyle(IA11yElement e)
        {
            var platformProperty = e?.GetPlatformPropertyValue<uint>(PlatformPropertyType.Platform_WindowsExtendedStylePropertyId);

            const uint WS_EX_STATICEDGE = 0x00020000;

            return (platformProperty & WS_EX_STATICEDGE) != 0;
        }

        private static bool IsPlatformWinForms(IA11yElement e)
        {
            return e?.Framework == Core.Enums.Framework.WinForm;
        }

        private static Condition CreateIsControlRequiredCondition()
        {
            return AppBar | Button | Calendar | CheckBox
                | ComboBox | DataGrid | DataItem | Document
                | Edit | Group | Header | HeaderItem
                | Hyperlink | Image | List | ListItem
                | Menu | MenuBar | MenuItem
                | ProgressBar | RadioButton | ScrollBar | SemanticZoom
                | Separator | Slider | Spinner | SplitButton
                | Tab | TabItem | Table
                | (Text & ~StringProperties.Framework.Is(Core.Enums.Framework.WPF)) 
                | Thumb | TitleBar | ToolBar
                | ToolTip | Tree | TreeItem | Window;
        }

        private static Condition CreateIsControlOptionalCondition()
        {
            return Pane | StatusBar;
        }

        private static Condition CreateAllowSameNameAndControlTypeCondition()
        {
            /* the following are generally found one per window/app
        * And therefore don't benefit from name-based distinction.
        * And for many, it's easy to imagine it being difficult to come up with a meaningful name beyond the control type. 
        * What else do you call a StatusBar for example?
        * Custom elements are handled separately because there may be times
        * when a name is "page 1" for example and the type is "page"
        * which we currently believe is acceptable because screen readers will read the name then the type
        * and "1 page" would sound awkward.
        * Names that are particularly long are viewed as acceptable because they are
        * more likely to be informative and not to cause double speaking by screen readers.
        * */

            var types = AppBar | Custom | Header | MenuBar | SemanticZoom | StatusBar | TitleBar | Text;

            return types | Name.Length > 50;
        }
    } // class
} // namespace
