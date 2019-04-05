// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.BoolProperties;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.LocalizedControlTypeNotNull)]
    class LocalizedControlTypeIsNotNull : Rule
    {
        public LocalizedControlTypeIsNotNull()
        {
            this.Info.Description = Descriptions.LocalizedControlTypeNotNull;
            this.Info.HowToFix = HowToFix.LocalizedControlTypeNotNull;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_LocalizedControlTypePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException("The element is null");

            return e.LocalizedControlType != null ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            return IsKeyboardFocusable & ~UWP.TitleBar;
        }
    } // class
} // namespace
