// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.Relationships;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.BoundingRectangleOnUWPMenuBar)]
    class BoundingRectangleOnUWPMenuBar : Rule
    {
        public BoundingRectangleOnUWPMenuBar()
        {
            this.Info.Description = Descriptions.BoundingRectangleOnUWPMenuBar;
            this.Info.HowToFix = HowToFix.BoundingRectangleOnUWPMenuBar;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_BoundingRectanglePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return BoundingRectangle.NotEmpty.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Open;
        }

        protected override Condition CreateCondition()
        {
            return UWP.MenuBar & Parent(UWP.TitleBar);
        }
    } // class
} // namespace
