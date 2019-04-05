// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.ControlType;
using static Axe.Windows.Rules.PropertyConditions.Relationships;
using static Axe.Windows.Rules.PropertyConditions.StringProperties;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.BoundingRectangleOnUWPMenuItem)]
    class BoundingRectangleOnUWPMenuItem : Rule
    {
        public BoundingRectangleOnUWPMenuItem()
        {
            this.Info.Description = Descriptions.BoundingRectangleOnUWPMenuItem;
            this.Info.HowToFix = HowToFix.BoundingRectangleOnUWPMenuItem;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_BoundingRectanglePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return BoundingRectangle.NotEmpty.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Open;
        }

        protected override Condition CreateCondition()
        {
            var grandparent = Ancestor(2, UWP.TitleBar);
            var parent = Parent(UWP.MenuBar & BoundingRectangle.Null);
            var self = MenuItem & Name.Is("System");
                return self & parent & grandparent;
        }
    } // class
} // namespace
