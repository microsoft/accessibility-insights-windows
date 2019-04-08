// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.ControlType;
using static Axe.Windows.Rules.PropertyConditions.Relationships;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.ItemTypeRecommended)]
    class ItemTypeRecommended : Rule
    {
        public ItemTypeRecommended()
        {
            this.Info.Description = Descriptions.ItemTypeRecommended;
            this.Info.HowToFix = HowToFix.ItemTypeRecommended;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_ItemTypePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return EvaluationCode.Open;
        }

        protected override Condition CreateCondition()
        {
            var recommendedControls = DataItem | ListItem | TreeItem;
            return recommendedControls & NoChild(HasSameType) & AnyChild(Image);
        }
    } // class
} // namespace
