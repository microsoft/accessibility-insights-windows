// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.BoundingRectangleNotNull)]
    class BoundingRectangleNotNull : Rule
    {
        public BoundingRectangleNotNull()
        {
            this.Info.ShortDescription = ShortDescriptions.BoundingRectangleNotNull;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = Core.Types.PropertyType.UIA_BoundingRectanglePropertyId;
            this.Info.Description = Descriptions.BoundingRectangleNotNull;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return BoundingRectangle.NotNull.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            var sysmenubar = ControlType.MenuBar & StringProperties.AutomationID.Is("SystemMenuBar") & StringProperties.Name.Is("System");
            var sysmenuitem = ControlType.MenuItem & Relationships.Parent(sysmenubar) & StringProperties.Name.Is("System");

            // the Bounding rectangle property might be empty due to
            // a non-existent property, or an invalid data format.
            // If the Bounding rectangle property is not empty, it means all the above criteria were met successfully
            // So we only want to test for null when the BoundingRectangle property is empty.
            //
            // 2 exceptions related with Menubar "System" and Menu item "System". 
            // These two are excluded since Windows 10 sets the bounding rectangles of these as null by default.
            return IsNotOffScreen
                & ~sysmenubar
                & ~sysmenuitem;
        }
    } // class
} // namespace
