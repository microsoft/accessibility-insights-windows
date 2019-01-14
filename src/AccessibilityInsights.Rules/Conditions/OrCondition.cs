// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Rules.Resources;

namespace AccessibilityInsights.Rules
{
    class OrCondition : Condition
    {
        private readonly Condition A;
        private readonly Condition B;

        public OrCondition(Condition a, Condition b)
        {
            if (a == null) throw new Exception("The A parameter to the OrConditionConstructor was null.");
            if (b == null) throw new Exception("The B parameter to the OrConditionConstructor was null.");

            this.A = a;
            this.B = b;
        }

        public override bool Matches(IA11yElement element)
        {
            return this.A.Matches(element)
                || this.B.Matches(element);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, ConditionDescriptions.Or, this.A.ToString(), this.B.ToString());
        }
    } // class
} // namespace
