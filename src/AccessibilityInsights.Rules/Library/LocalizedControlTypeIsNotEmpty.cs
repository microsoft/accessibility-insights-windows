// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.StringProperties;
using static Axe.Windows.Rules.PropertyConditions.BoolProperties;


namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.LocalizedControlTypeNotEmpty)]
    class LocalizedControlTypeIsNotEmpty : Rule
    {
        public LocalizedControlTypeIsNotEmpty()
        {
            this.Info.Description = Descriptions.LocalizedControlTypeNotEmpty;
            this.Info.HowToFix = HowToFix.LocalizedControlTypeNotEmpty;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_LocalizedControlTypePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException("The element is null");

            return LocalizedControlType.NotEmpty.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            return IsKeyboardFocusable & LocalizedControlType.NotNull;
        }
    } // class
} // namespace
