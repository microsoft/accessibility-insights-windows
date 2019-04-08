// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.StringProperties;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.HelpTextNotEqualToName)]
    class HelpTextNotEqualToName : Rule
    {
        public HelpTextNotEqualToName()
        {
            this.Info.Description = Descriptions.HelpTextNotEqualToName;
            this.Info.HowToFix = HowToFix.HelpTextNotEqualToName;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            var condition = Name.IsEqualTo(HelpText);
            return condition.Matches(e) ? EvaluationCode.Warning : EvaluationCode.Pass;
        }

        protected override Condition CreateCondition()
        {
            return Name.NotNullOrEmpty & HelpText.NotNullOrEmpty;
        }
    } // class
} // namespace
