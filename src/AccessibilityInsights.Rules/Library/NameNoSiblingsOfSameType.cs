// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.ControlType;
using static Axe.Windows.Rules.PropertyConditions.Relationships;
using static Axe.Windows.Rules.PropertyConditions.StringProperties;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.NameNoSiblingsOfSameType)]
    class NameNoSiblingsOfSameType : Rule
    {
        public NameNoSiblingsOfSameType()
        {
            this.Info.Description = Descriptions.NameNoSiblingsOfSameType;
            this.Info.HowToFix = HowToFix.NameNoSiblingsOfSameType;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_NamePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return EvaluationCode.Note;
        }

        protected override Condition CreateCondition()
        {
            var control = Header | StatusBar | Tab;
            return control & Name.NullOrEmpty & NoSiblingsOfSameType;
        }
    } // class
} // namespace
