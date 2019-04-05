// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Text;

namespace Axe.Windows.Core.Types
{
    /// <summary>
    /// Class for Element Property Type IDs
    /// List is built based on UIAutomationClient.h
    /// Any of pattern property should be modifiled like below. 
    /// 
    /// UIA_ValueValuePropertyId => UIA_ValuePattern_ValuePropertyId
    /// 
    /// Basically, between Pattern name and PropertyName, you need to put "Pattern_" to make name more aligned. 
    /// </summary>
    public class PropertyType:TypeBase
    {
#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const int UIA_RuntimeIdPropertyId = 30000;
        public const int UIA_BoundingRectanglePropertyId = 30001;
        public const int UIA_ProcessIdPropertyId = 30002;
        public const int UIA_ControlTypePropertyId = 30003;
        public const int UIA_LocalizedControlTypePropertyId = 30004;
        public const int UIA_NamePropertyId = 30005;
        public const int UIA_AcceleratorKeyPropertyId = 30006;
        public const int UIA_AccessKeyPropertyId = 30007;
        public const int UIA_HasKeyboardFocusPropertyId = 30008;
        public const int UIA_IsKeyboardFocusablePropertyId = 30009;
        public const int UIA_IsEnabledPropertyId = 30010;
        public const int UIA_AutomationIdPropertyId = 30011;
        public const int UIA_ClassNamePropertyId = 30012;
        public const int UIA_HelpTextPropertyId = 30013;
        public const int UIA_ClickablePointPropertyId = 30014;
        public const int UIA_CulturePropertyId = 30015;
        public const int UIA_IsControlElementPropertyId = 30016;
        public const int UIA_IsContentElementPropertyId = 30017;
        public const int UIA_LabeledByPropertyId = 30018;
        public const int UIA_IsPasswordPropertyId = 30019;
        public const int UIA_NativeWindowHandlePropertyId = 30020;
        public const int UIA_ItemTypePropertyId = 30021;
        public const int UIA_IsOffscreenPropertyId = 30022;
        public const int UIA_OrientationPropertyId = 30023;
        public const int UIA_FrameworkIdPropertyId = 30024;
        public const int UIA_IsRequiredForFormPropertyId = 30025;
        public const int UIA_ItemStatusPropertyId = 30026;
        public const int UIA_IsDockPatternAvailablePropertyId = 30027;
        public const int UIA_IsExpandCollapsePatternAvailablePropertyId = 30028;
        public const int UIA_IsGridItemPatternAvailablePropertyId = 30029;
        public const int UIA_IsGridPatternAvailablePropertyId = 30030;
        public const int UIA_IsInvokePatternAvailablePropertyId = 30031;
        public const int UIA_IsMultipleViewPatternAvailablePropertyId = 30032;
        public const int UIA_IsRangeValuePatternAvailablePropertyId = 30033;
        public const int UIA_IsScrollPatternAvailablePropertyId = 30034;
        public const int UIA_IsScrollItemPatternAvailablePropertyId = 30035;
        public const int UIA_IsSelectionItemPatternAvailablePropertyId = 30036;
        public const int UIA_IsSelectionPatternAvailablePropertyId = 30037;
        public const int UIA_IsTablePatternAvailablePropertyId = 30038;
        public const int UIA_IsTableItemPatternAvailablePropertyId = 30039;
        public const int UIA_IsTextPatternAvailablePropertyId = 30040;
        public const int UIA_IsTogglePatternAvailablePropertyId = 30041;
        public const int UIA_IsTransformPatternAvailablePropertyId = 30042;
        public const int UIA_IsValuePatternAvailablePropertyId = 30043;
        public const int UIA_IsWindowPatternAvailablePropertyId = 30044;
        public const int UIA_ValuePattern_ValuePropertyId = 30045;
        public const int UIA_ValuePattern_IsReadOnlyPropertyId = 30046;
        public const int UIA_RangeValuePattern_ValuePropertyId = 30047;
        public const int UIA_RangeValuePattern_IsReadOnlyPropertyId = 30048;
        public const int UIA_RangeValuePattern_MinimumPropertyId = 30049;
        public const int UIA_RangeValuePattern_MaximumPropertyId = 30050;
        public const int UIA_RangeValuePattern_LargeChangePropertyId = 30051;
        public const int UIA_RangeValuePattern_SmallChangePropertyId = 30052;
        public const int UIA_ScrollPattern_HorizontalScrollPercentPropertyId = 30053;
        public const int UIA_ScrollPattern_HorizontalViewSizePropertyId = 30054;
        public const int UIA_ScrollPattern_VerticalScrollPercentPropertyId = 30055;
        public const int UIA_ScrollPattern_VerticalViewSizePropertyId = 30056;
        public const int UIA_ScrollPattern_HorizontallyScrollablePropertyId = 30057;
        public const int UIA_ScrollPattern_VerticallyScrollablePropertyId = 30058;
        public const int UIA_SelectionPattern_SelectionPropertyId = 30059;
        public const int UIA_SelectionPattern_CanSelectMultiplePropertyId = 30060;
        public const int UIA_SelectionPattern_IsSelectionRequiredPropertyId = 30061;
        public const int UIA_GridPattern_RowCountPropertyId = 30062;
        public const int UIA_GridPattern_ColumnCountPropertyId = 30063;
        public const int UIA_GridItemPattern_RowPropertyId = 30064;
        public const int UIA_GridItemPattern_ColumnPropertyId = 30065;
        public const int UIA_GridItemPattern_RowSpanPropertyId = 30066;
        public const int UIA_GridItemPattern_ColumnSpanPropertyId = 30067;
        public const int UIA_GridItemPattern_ContainingGridPropertyId = 30068;
        public const int UIA_DockDockPositionPropertyId = 30069;
        public const int UIA_ExpandCollapseExpandCollapseStatePropertyId = 30070;
        public const int UIA_MultipleViewPattern_CurrentViewPropertyId = 30071;
        public const int UIA_MultipleViewPattern_SupportedViewsPropertyId = 30072;
        public const int UIA_WindowPattern_CanMaximizePropertyId = 30073;
        public const int UIA_WindowPattern_CanMinimizePropertyId = 30074;
        public const int UIA_WindowPattern_WindowVisualStatePropertyId = 30075;
        public const int UIA_WindowPattern_WindowInteractionStatePropertyId = 30076;
        public const int UIA_WindowPattern_IsModalPropertyId = 30077;
        public const int UIA_WindowPattern_IsTopmostPropertyId = 30078;
        public const int UIA_SelectionItemPattern_IsSelectedPropertyId = 30079;
        public const int UIA_SelectionItemPattern_SelectionContainerPropertyId = 30080;
        public const int UIA_TablePattern_RowHeadersPropertyId = 30081;
        public const int UIA_TablePattern_ColumnHeadersPropertyId = 30082;
        public const int UIA_TablePattern_RowOrColumnMajorPropertyId = 30083;
        public const int UIA_TableItemPattern_RowHeaderItemsPropertyId = 30084;
        public const int UIA_TableItemPattern_ColumnHeaderItemsPropertyId = 30085;
        public const int UIA_TogglePattern_ToggleStatePropertyId = 30086;
        public const int UIA_TransformPattern_CanMovePropertyId = 30087;
        public const int UIA_TransformPattern_CanResizePropertyId = 30088;
        public const int UIA_TransformPattern_CanRotatePropertyId = 30089;
        public const int UIA_IsLegacyIAccessiblePatternAvailablePropertyId = 30090;
        public const int UIA_LegacyIAccessiblePattern_ChildIdPropertyId = 30091;
        public const int UIA_LegacyIAccessiblePattern_NamePropertyId = 30092;
        public const int UIA_LegacyIAccessiblePattern_ValuePropertyId = 30093;
        public const int UIA_LegacyIAccessiblePattern_DescriptionPropertyId = 30094;
        public const int UIA_LegacyIAccessiblePattern_RolePropertyId = 30095;
        public const int UIA_LegacyIAccessiblePattern_StatePropertyId = 30096;
        public const int UIA_LegacyIAccessiblePattern_HelpPropertyId = 30097;
        public const int UIA_LegacyIAccessiblePattern_KeyboardShortcutPropertyId = 30098;
        public const int UIA_LegacyIAccessiblePattern_SelectionPropertyId = 30099;
        public const int UIA_LegacyIAccessiblePattern_DefaultActionPropertyId = 30100;
        public const int UIA_AriaRolePropertyId = 30101;
        public const int UIA_AriaPropertiesPropertyId = 30102;
        public const int UIA_IsDataValidForFormPropertyId = 30103;
        public const int UIA_ControllerForPropertyId = 30104;
        public const int UIA_DescribedByPropertyId = 30105;
        public const int UIA_FlowsToPropertyId = 30106;
        public const int UIA_ProviderDescriptionPropertyId = 30107;
        public const int UIA_IsItemContainerPatternAvailablePropertyId = 30108;
        public const int UIA_IsVirtualizedItemPatternAvailablePropertyId = 30109;
        public const int UIA_IsSynchronizedInputPatternAvailablePropertyId = 30110;
        public const int UIA_OptimizeForVisualContentPropertyId = 30111;
        public const int UIA_IsObjectModelPatternAvailablePropertyId = 30112;
        public const int UIA_AnnotationPattern_AnnotationTypeIdPropertyId = 30113;
        public const int UIA_AnnotationPattern_AnnotationTypeNamePropertyId = 30114;
        public const int UIA_AnnotationPattern_AuthorPropertyId = 30115;
        public const int UIA_AnnotationPattern_DateTimePropertyId = 30116;
        public const int UIA_AnnotationPattern_TargetPropertyId = 30117;
        public const int UIA_IsAnnotationPatternAvailablePropertyId = 30118;
        public const int UIA_IsTextPattern2AvailablePropertyId = 30119;
        public const int UIA_StylesPattern_StyleIdPropertyId = 30120;
        public const int UIA_StylesPattern_StyleNamePropertyId = 30121;
        public const int UIA_StylesPattern_FillColorPropertyId = 30122;
        public const int UIA_StylesPattern_FillPatternStylePropertyId = 30123;
        public const int UIA_StylesPattern_ShapePropertyId = 30124;
        public const int UIA_StylesPattern_FillPatternColorPropertyId = 30125;
        public const int UIA_StylesPattern_ExtendedPropertiesPropertyId = 30126;
        public const int UIA_IsStylesPatternAvailablePropertyId = 30127;
        public const int UIA_IsSpreadsheetPatternAvailablePropertyId = 30128;
        public const int UIA_SpreadsheetItemPatternFormulaPropertyId = 30129;
        public const int UIA_SpreadsheetItemPattern_AnnotationObjectsPropertyId = 30130;
        public const int UIA_SpreadsheetItemPattern_AnnotationTypesPropertyId = 30131;
        public const int UIA_IsSpreadsheetItemPatternAvailablePropertyId = 30132;
        public const int UIA_Transform2Pattern_CanZoomPropertyId = 30133;
        public const int UIA_IsTransformPattern2AvailablePropertyId = 30134;
        public const int UIA_LiveSettingPropertyId = 30135;
        public const int UIA_IsTextChildPatternAvailablePropertyId = 30136;
        public const int UIA_IsDragPatternAvailablePropertyId = 30137;
        public const int UIA_DragPattern_IsGrabbedPropertyId = 30138;
        public const int UIA_DragPattern_DropEffectPropertyId = 30139;
        public const int UIA_DragPattern_DropEffectsPropertyId = 30140;
        public const int UIA_IsDropTargetPatternAvailablePropertyId = 30141;
        public const int UIA_DropTargetPattern_DropTargetEffectPropertyId = 30142;
        public const int UIA_DropTargetPattern_DropTargetEffectsPropertyId = 30143;
        public const int UIA_DragGrabbedItemsPropertyId = 30144;
        public const int UIA_Transform2Pattern_ZoomLevelPropertyId = 30145;
        public const int UIA_Transform2Pattern_ZoomMinimumPropertyId = 30146;
        public const int UIA_Transform2Pattern_ZoomMaximumPropertyId = 30147;
        public const int UIA_FlowsFromPropertyId = 30148;
        public const int UIA_IsTextEditPatternAvailablePropertyId = 30149;
        public const int UIA_IsPeripheralPropertyId = 30150;
        public const int UIA_IsCustomNavigationPatternAvailablePropertyId = 30151;
        public const int UIA_PositionInSetPropertyId = 30152;
        public const int UIA_SizeOfSetPropertyId = 30153;
        public const int UIA_LevelPropertyId = 30154;
        public const int UIA_AnnotationPattern_TypesPropertyId = 30155;
        public const int UIA_AnnotationPattern_ObjectsPropertyId = 30156;
        public const int UIA_LandmarkTypePropertyId = 30157;
        public const int UIA_LocalizedLandmarkTypePropertyId = 30158;
        public const int UIA_FullDescriptionPropertyId = 30159;
        public const int UIA_FillColorPropertyId = 30160;
        public const int UIA_OutlineColorPropertyId = 30161;
        public const int UIA_FillTypePropertyId = 30162;
        public const int UIA_VisualEffectsPropertyId = 30163;
        public const int UIA_OutlineThicknessPropertyId = 30164;
        public const int UIA_CenterPointPropertyId = 30165;
        public const int UIA_RotationPropertyId = 30166;
        public const int UIA_SizePropertyId = 30167;
        public const int UIA_HeadingLevelPropertyId = 30173;
        public const int UIA_IsDialogPropertyId = 30174;
#pragma warning restore CA1707 // Identifiers should not contain underscores

        private static PropertyType sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static PropertyType GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new PropertyType();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private PropertyType() : base() { }

        /// <summary>
        /// change name into right format in dictionary and list.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override string GetNameInProperFormat(string name, int id)
        {
            StringBuilder sb = new StringBuilder(name);

            sb.Replace("UIA_", "");
            sb.Replace("PropertyId", "");
            sb.Replace("_", ".");

            return sb.ToString();
        }
    }
}
