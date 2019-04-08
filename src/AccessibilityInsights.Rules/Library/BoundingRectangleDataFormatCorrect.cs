// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.BoolProperties;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.BoundingRectangleDataFormatCorrect)]
    class BoundingRectangleDataFormatCorrect : Rule
    {
        public BoundingRectangleDataFormatCorrect()
        {
            this.Info.Description = Descriptions.BoundingRectangleDataFormatCorrect;
            this.Info.HowToFix = HowToFix.BoundingRectangleDataFormatCorrect;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_BoundingRectanglePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return BoundingRectangle.CorrectDataFormat.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            // the Bounding rectangle property might be empty due to
            // a non-existent property, or an invalid data format.
            // If the Bounding rectangle property is not empty, it means all the above criteria were met successfully
            // So we only want to test for format when the BoundingRectangle property is empty.
            return IsNotOffScreen & BoundingRectangle.Empty & BoundingRectangle.NotNull;
        }
    } // class
} // namespace
