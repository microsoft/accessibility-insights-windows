// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;
using static AccessibilityInsights.Rules.PropertyConditions.Relationships;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.IsContentElementTrueOptional)]
    class IsContentElementTrueOptional : Rule
    {
        public IsContentElementTrueOptional()
        {
            this.Info.Description = Descriptions.IsContentElementTrueOptional;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_IsContentElementPropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return EvaluationCode.Open;
        }

        protected override Condition CreateCondition()
        {
            var controls =  (Button & NotParent(ComboBox | ScrollBar | TitleBar))
                | Calendar | CheckBox | ComboBox | DataGrid | DataItem
                | Document | Edit | Group
                | Hyperlink | List | ListItem | MenuItem
                | Pane | ProgressBar | RadioButton | SemanticZoom
                | Slider | Spinner | SplitButton | StatusBar | Tab
                | TabItem | Table | ToolBar
                | Tree | TreeItem | Window;

            return IsNotContentElement & controls & BoundingRectangle.Valid;
        }
    } // class
} // namespace
