// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.BoolProperties;
using static Axe.Windows.Rules.PropertyConditions.ControlType;
using static Axe.Windows.Rules.PropertyConditions.Relationships;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.BoundingRectangleCompletelyObscuresContainer)]
    class BoundingRectangleCompletelyObscuresContainer : Rule
    {
        public BoundingRectangleCompletelyObscuresContainer()
        {
            this.Info.Description = Descriptions.BoundingRectangleCompletelyObscuresContainer;
            this.Info.HowToFix = HowToFix.BoundingRectangleCompletelyObscuresContainer;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_BoundingRectanglePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return BoundingRectangle.CompletelyObscuresContainer.Matches(e) ? EvaluationCode.Error : EvaluationCode.Pass;
        }

        protected override Condition CreateCondition()
        {
            // Windows can be any size, regardless of their parents

            return ~Window
                & IsNotOffScreen
                & BoundingRectangle.Valid
                & ParentExists
                & Parent(IsNotDesktop)
                & Parent(BoundingRectangle.Valid);
        }
    } // class
} // namespace
