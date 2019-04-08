// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Text.RegularExpressions;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.StringProperties;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.NameExcludesLocalizedControlType)]
    class NameExcludesLocalizedControlType: Rule
    {
        public NameExcludesLocalizedControlType()
        {
            this.Info.Description = Descriptions.NameExcludesLocalizedControlType;
            this.Info.HowToFix = HowToFix.NameExcludesLocalizedControlType;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_NamePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));
            if (e.Name == null) throw new ArgumentException(nameof(e.Name));
            if (e.LocalizedControlType == null) throw new ArgumentException(nameof(e.LocalizedControlType));

            var r = new Regex($@"\b{e.LocalizedControlType}\b", RegexOptions.IgnoreCase);

            return r.IsMatch(e.Name)
                ? EvaluationCode.Error : EvaluationCode.Pass;
        }

        protected override Condition CreateCondition()
        {
            return ~ElementGroups.AllowSameNameAndControlType
                & Name.NotNullOrEmpty & Name.NotWhiteSpace
                & StringProperties.LocalizedControlType.NotNullOrEmpty & StringProperties.LocalizedControlType.NotWhiteSpace;
        }
    } // class
} // namespace
