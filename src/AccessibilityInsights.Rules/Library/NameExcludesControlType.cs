// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Text.RegularExpressions;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.Misc;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.StringProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.NameExcludesControlType)]
    class NameExcludesControlType : Rule
    {
        public NameExcludesControlType()
        {
            this.Info.Description = Descriptions.NameExcludesControlType;
            this.Info.Standard = A11yCriteriaId.ObjectInformation;
            this.Info.PropertyID = PropertyType.UIA_NamePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));
            if (String.IsNullOrWhiteSpace(e.Name)) throw new ArgumentException($"{nameof(e.Name)} was null or white space");

            if (!ControlTypeStrings.Dictionary.TryGetValue(e.ControlTypeId, out string controlTypeString)) throw new InvalidOperationException($"No control type entry was found in {nameof(ControlTypeStrings.Dictionary)}");

            var r = new Regex($@"\b{controlTypeString}\b", RegexOptions.IgnoreCase);

            return r.IsMatch(e.Name)
                ? EvaluationCode.Error : EvaluationCode.Pass;
        }

        protected override Condition CreateCondition()
        {
            return ~ElementGroups.AllowSameNameAndControlType
                & Name.NotNullOrEmpty
                & Name.NotWhiteSpace;
        }
    } // class
} // namespace
