// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.BoolProperties;
using static Axe.Windows.Rules.PropertyConditions.ControlType;
using static Axe.Windows.Rules.PropertyConditions.Relationships;
using static Axe.Windows.Rules.PropertyConditions.StringProperties;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.BoundingRectangleSizeReasonable)]
    class BoundingRectangleSizeReasonable : Rule
    {
        public BoundingRectangleSizeReasonable()
        {
            this.Info.Description = Descriptions.BoundingRectangleSizeReasonable;
            this.Info.HowToFix = HowToFix.BoundingRectangleSizeReasonable;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_BoundingRectanglePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return BoundingRectangle.Valid.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            var ignoreableText = Text & ~IsKeyboardFocusable & Name.NullOrEmpty & ~ChildrenExist;

            return IsNotOffScreen
                & BoundingRectangle.NotNull
                & BoundingRectangle.CorrectDataFormat
                & ~ignoreableText;
        }
    } // class
} // namespace
