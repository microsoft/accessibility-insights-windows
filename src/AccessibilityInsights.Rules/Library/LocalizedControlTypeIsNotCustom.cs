// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.BoolProperties;
using static Axe.Windows.Rules.PropertyConditions.ControlType;
using static Axe.Windows.Rules.PropertyConditions.Relationships;
using static Axe.Windows.Rules.PropertyConditions.StringProperties;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.LocalizedControlTypeNotCustom)]
    class LocalizedControlTypeIsNotCustom : Rule
    {
        public LocalizedControlTypeIsNotCustom()
        {
            this.Info.Description = Descriptions.LocalizedControlTypeNotCustom;
            this.Info.HowToFix = HowToFix.LocalizedControlTypeNotCustom;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_LocalizedControlTypePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException("The element is null");

            return e.LocalizedControlType != LocalizedControlTypeNames.Custom ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            var dataGridDetailsPresenter = ClassName.Is("DataGridDetailsPresenter") & Parent(DataItem);

            return Custom
                & IsKeyboardFocusable
                & LocalizedControlType.NotNullOrEmpty
                & ~dataGridDetailsPresenter;
        }
    } // class
} // namespace
