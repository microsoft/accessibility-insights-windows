// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.ControlType;
using static Axe.Windows.Rules.PropertyConditions.IntProperties;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.OrientationPropertyExists)]
    class OrientationPropertyExists : Rule
    {
        public OrientationPropertyExists()
        {
            this.Info.Description = Descriptions.OrientationPropertyExists;
            this.Info.HowToFix = HowToFix.OrientationPropertyExists;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_OrientationPropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return Orientation.Exists.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            return ScrollBar | Tab;
        }
    } // class
} // namespace
