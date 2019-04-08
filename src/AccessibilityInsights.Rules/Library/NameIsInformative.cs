// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Text.RegularExpressions;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.StringProperties;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.NameIsInformative)]
    class NameIsInformative : Rule
    {
        public NameIsInformative()
        {
            this.Info.Description = Descriptions.NameIsInformative;
            this.Info.HowToFix = HowToFix.NameIsInformative;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_NamePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            string[] stringsToExclude =
            {
                @"^\s*Microsoft(\.(\w|\d)+)+\s*$",
                @"^\s*Windows(\.(\w|\d)+)+\s*$"
            };

            foreach (var s in stringsToExclude)
            {
                if (Regex.IsMatch(e.Name, s, RegexOptions.IgnoreCase))
                    return EvaluationCode.Error;
            }

            return EvaluationCode.Pass;
        }

        protected override Condition CreateCondition()
        {
            return Name.NotNullOrEmpty & Name.NotWhiteSpace & BoundingRectangle.Valid
                & ( ElementGroups.NameRequired | ElementGroups.NameOptional);
        }
    } // class
} // namespace
