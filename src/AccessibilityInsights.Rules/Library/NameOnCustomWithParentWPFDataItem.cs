// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.ControlType;
using static Axe.Windows.Rules.PropertyConditions.StringProperties;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.NameOnCustomWithParentWPFDataItem)]
    class NameOnCustomWithParentWPFDataItem: Rule
    {
        public NameOnCustomWithParentWPFDataItem()
        {
            this.Info.Description = Descriptions.NameOnCustomWithParentWPFDataItem;
            this.Info.HowToFix = HowToFix.NameOnCustomWithParentWPFDataItem;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_NamePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return EvaluationCode.Note;
        }

        protected override Condition CreateCondition()
        {
            return Custom & Name.NullOrEmpty & ElementGroups.ParentWPFDataItem;
        }
    } // class
} // namespace
