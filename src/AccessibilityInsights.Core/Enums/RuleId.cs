// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Axe.Windows.Core.Enums
{
    /// <summary>
    /// Strong rule ids. Previously rules were only identifiable through
    ///     their string description values
    /// Rules are grouped by scan via whitespace when relevant
    /// If writing a ScanException, generally the first rule within a scan grouping
    ///     is appropriate to use in the constructor of GetRuleResultInstance(...)
    /// 
    /// Please don't remove or rename any existing ruleIDs. 
    /// since RuleIds are serialized in string format, if any name is changed or removed, it may cause an error while deserializing an existing reesults file.  
    /// </summary>
    public enum RuleId
    {
        // this value will be set if the RuleResult.Rule is loaded from Old results file. 
        // it happens since we removed RuleId propert and replace it with Rule which is serialized in string format.
        Indecisive = 0, 

        ScanMutedByException,

        // scans
        BoundingRectangleExists,

        // Axe.Windows.Rules
        BoundingRectangleNotAllZeros,
        BoundingRectangleNotNull,
        BoundingRectangleNotValidButOffScreen,
        BoundingRectangleDataFormatCorrect,
        BoundingRectangleCompletelyObscuresContainer,
        BoundingRectangleContainedInParent,
        BoundingRectangleOnUWPMenuBar,
        BoundingRectangleOnUWPMenuItem,
        BoundingRectangleOnWPFTextParent,
        BoundingRectangleSizeReasonable,

        // Axe.Windows.Rules
        SplitButtonInvokeAndTogglePatterns,
        ButtonShouldHavePatterns, // check whether button has at least one of three patterns(Invoke,Toggle,ExpandCollapse)
        ButtonInvokeAndTogglePatterns, // Button should not have Invoke and Toggle patterns together. 
        ButtonInvokeAndExpandCollapsePatterns, // Button may have Invoke and ExpandCollapse patterns together. (warning)
        ButtonToggleAndExpandCollapsePatterns, // Button should have have Toggle and ExpandCollapse patterns together. 
        ButtonExpandCollapsePattern,   // obsolete but don't remove since removal will break backward compatibility.  
        ButtonPatterns, // obsolete. keep it to avoid backward compat issue. 

        // scans
        ChildUniqueNameOrType,

        // Axe.Windows.Rules
        ChildUniqueNameOrTypeAndFocusable, // deprecated
        ChildUniqueNameOrTypeAndNotFocusable, // deprecated
        SiblingUniqueAndFocusable,
        SiblingUniqueAndNotFocusable,

        // Axe.Windows.Rules
        ChildrenNotAllowedInContentView,

        // Axe.Windows.Rules
        ContentViewButtonStructure,
        ContentViewCalendarStructure,
        ContentViewCheckBoxStructure,
        ContentViewComboBoxStructure,
        ContentViewDataGridStructure,
        ContentViewEditStructure,
        ContentViewHyperlinkStructure,
        ContentViewListStructure,
        ContentViewListItemStructure,
        ContentViewMenuStructure,
        ContentViewProgressBarStructure,
        ContentViewRadioButtonStructure,
        ContentViewSliderStructure,
        ContentViewSpinnerStructure,
        ContentViewSplitButtonStructure,
        ContentViewStatusBarStructure,
        ContentViewTabStructure,
        ContentViewTreeStructure,
        ContentViewTreeItemStructure,
        ControlViewButtonStructure,
        ControlViewCalendarStructure,
        ControlViewComboBoxStructure,
        ControlViewCheckBoxStructure,
        ControlViewDataGridStructure,
        ControlViewEditStructure,
        ControlViewHeaderStructure,
        ControlViewHeaderItemStructure,
        ControlViewHyperlinkStructure,
        ControlViewImageStructure,
        ControlViewListStructure,
        ControlViewListItemStructure,
        ControlViewMenuStructure,
        ControlViewProgressBarStructure,
        ControlViewRadioButtonStructure,
        ControlViewScrollbarStructure,
        ControlViewSemanticZoomStructure,
        ControlViewSeparatorStructure,
        ControlViewSliderStructure,
        ControlViewSpinnerStructure,
        ControlViewSplitButtonStructure,
        ControlViewStatusBarStructure,
        ControlViewTabStructure,
        ControlViewThumbStructure,
        ControlViewToolTipStructure,
        ControlViewTreeStructure,
        ControlViewTreeItemStructure,

        // Axe.Windows.Rules
        ComboBoxShouldNotSupportScrollPattern,
        ControlShouldNotSupportInvokePattern,
        ControlShouldNotSupportScrollPattern,
        ControlShouldNotSupportTablePattern,
        ControlShouldNotSupportTogglePattern,
        ControlShouldNotSupportValuePattern,
        ControlShouldNotSupportWindowPattern,
        ControlShouldSupportExpandCollapsePattern,
        ControlShouldSupportGridItemPattern,
        ControlShouldSupportGridPattern,
        ControlShouldSupportInvokePattern,
        ControlShouldSupportScrollItemPattern,
        ControlShouldSupportSelectionItemPattern,
        ControlShouldSupportSelectionPattern,
        ControlShouldSupportSetInfo,
        ControlShouldSupportSpreadsheetItemPattern,
        ControlShouldSupportTableItemPattern,
        ControlShouldSupportTablePattern,
        ControlShouldSupportTogglePattern,
        ControlShouldSupportTransformPattern,
        ControlShouldSupportTextPattern,

        // Axe.Windows.Rules
        EditSupportsIncorrectRangeValuePattern,
        EditSupportsOnlyValuePattern,
        HeadingLevelDescendsWhenNested,

        // Axe.Windows.Rules
        LandmarkBannerIsTopLevel,
        LandmarkComplementaryIsTopLevel,
        LandmarkContentInfoIsTopLevel,
        LandmarkMainIsTopLevel,
        LandmarkNoDuplicateBanner,
        LandmarkNoDuplicateContentInfo,
        LandmarkOneMain,

        LocalizedLandmarkTypeExcludesSpecialCharacters,
        LocalizedLandmarkTypeIsReasonableLength,
        LocalizedLandmarkTypeNotCustom,
        LocalizedLandmarkTypeNotEmpty,
        LocalizedLandmarkTypeNotNull,
        LocalizedLandmarkTypeNotWhiteSpace,

        PatternsSupportedByControlType,

        PatternsExpectedBasedOnParent,

        PatternsExpectedBasedOnControlType,

        HelpTextExists,

        // Axe.Windows.Rules
        HelpTextNotEqualToName,
        IsControlElementPropertyExists,
        IsControlElementPropertyCorrect,

        // Axe.Windows.Rules
        IsContentElementPropertyExists,
        IsContentElementFalseOptional,
        IsContentElementTrueOptional,
        IsControlElementTrueOptional,
        IsControlElementTrueRequired,

        // Scans
        IsKeyboardFocusable,
        IsKeyboardFocusableBasedOnPatterns,

        // Axe.Windows.Rules
        IsKeyboardFocusableShouldBeTrue,
        IsKeyboardFocusableFalseButDisabled,
        IsKeyboardFocusableForListItemShouldBeTrue,
        IsKeyboardFocusableFalseButOffscreen,
        IsKeyboardFocusableForCustomShouldBeTrue,
        IsKeyboardFocusableDescendantTextPattern,
        IsKeyboardFocusableOnEmptyContainer,
        IsKeyboardFocusableShouldBeFalse,
        IsKeyboardFocusableTopLevelTextPattern,

        ItemTypeCorrect,

        // Axe.Windows.Rules
        ItemTypeRecommended,

        // Axe.Windows.Rules
        LocalizedControlTypeExists,
        LocalizedControlTypeReasonable,

        // scans
        NameNonEmpty,

        // Axe.Windows.Rules
        NameNotEmpty,
        NameExcludesControlType,
        NameExcludesLocalizedControlType,
        NameExcludesSpecialCharacters,
        NameReasonableLength,

        OrientationPropertyExists,
        ProgressBarRangeValue,

        IncludesWebContent,

        // These come from the "ScanPropertyAttribute" scan, which
        // are used in Scanner.Controls in a dynamic way, e.g.
        // [ScanProperty(PropertyTypes.UIA_IsContentElementPropertyId, ExpectedValue = false, ...
        // But each such property will require its own rule id here.
        // given by ExtensionMethods.GetPropertyCorrectRule(int A11yProperty)
        IsContentElementPropertyCorrect,
        ItemStatusPropertyCorrect,

        // Axe.Windows.Rules
        ItemStatusExists,

        // given by ExtensionMethods.GetTreeStructureRule(TreeViewModes)
        TypicalTreeStructureRaw,
        TypicalTreeStructureControl,
        TypicalTreeStructureContent,

        NameNotNull,
        NameNotWhiteSpace,
        NameNullButElementNotKeyboardFocusable,
        NameEmptyButElementNotKeyboardFocusable,
        NameWithValidBoundingRectangle,
        NameOnOptionalType,
        NameNoSiblingsOfSameType,
        NameOnCustomWithParentWPFDataItem,
        NameIsInformative,

        LocalizedControlTypeNotWhiteSpace,
        LocalizedControlTypeNotEmpty,
        LocalizedControlTypeNotNull,
        LocalizedControlTypeNotCustom,

        ParentChildShouldNotHaveSameNameAndLocalizedControlType,

        // Axe.Windows.Rules
        SelectionPatternSelectionRequired,
        SelectionPatternSingleSelection,
        SelectionItemPatternSingleSelection,

        ListItemSiblingsUnique,
    }
}
