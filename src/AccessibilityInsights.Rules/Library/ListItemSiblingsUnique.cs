// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Rules.PropertyConditions;
using Axe.Windows.Rules.Resources;
using System;
using static Axe.Windows.Rules.PropertyConditions.BoolProperties;
using static Axe.Windows.Rules.PropertyConditions.ControlType;
using static Axe.Windows.Rules.PropertyConditions.Relationships;
using static Axe.Windows.Rules.PropertyConditions.StringProperties;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.ListItemSiblingsUnique)]
    class ListItemSiblingsUnique : Rule
    {
        public ListItemSiblingsUnique()
        {
            this.Info.Description = Descriptions.ListItemSiblingsUnique;
            this.Info.HowToFix = HowToFix.ListItemSiblingsUnique;
            this.Info.Standard = A11yCriteriaId.NameRoleValue;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));
            if (e.Parent == null) throw new ArgumentNullException(nameof(e.Parent));

            var siblings = SiblingCount(new ControlTypeCondition(e.ControlTypeId)
                & (e.IsKeyboardFocusable ? IsKeyboardFocusable : IsNotKeyboardFocusable)
                & Name.Is(e.Name)
                & LocalizedControlType.Is(e.LocalizedControlType));
            var count = siblings.GetValue(e);
            if (count < 1) return EvaluationCode.RuleExecutionError;

            return count == 1 ? EvaluationCode.Pass : EvaluationCode.Warning;
        }

        protected override Condition CreateCondition()
        {
            return ListItem
                & IsContentOrControlElement
                & ParentExists
                & Name.NotNullOrEmpty
                & LocalizedControlType.NotNullOrEmpty
                & BoundingRectangle.Valid;
        }
    } // class
} // namespace
