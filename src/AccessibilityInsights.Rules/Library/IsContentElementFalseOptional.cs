// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.BoolProperties;
using static Axe.Windows.Rules.PropertyConditions.ControlType;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.IsContentElementFalseOptional)]
    class IsContentElementFalseOptional : Rule
    {
        public IsContentElementFalseOptional()
        {

            this.Info.Description = Descriptions.IsContentElementFalseOptional;
            this.Info.HowToFix = HowToFix.IsContentElementFalseOptional;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_IsContentElementPropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return EvaluationCode.Open;
        }

        protected override Condition CreateCondition()
        {
            var controls = AppBar | Header | HeaderItem | MenuBar
                | ScrollBar | Separator | Thumb | TitleBar;

            return IsContentElement & controls & BoundingRectangle.Valid;
        }
    } // class
} // namespace
