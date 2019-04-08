// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
using Axe.Windows.Core.Bases;
using Axe.Windows.Rules.Resources;

namespace Axe.Windows.Rules
{
    class AndCondition : Condition
    {
        private readonly Condition A;
        private readonly Condition B;

        public AndCondition(Condition a, Condition b)
        {
            if (a == null) throw new ArgumentException();
            if (b == null) throw new ArgumentException();

            this.A = a;
            this.B = b;
        }

        public override bool Matches(IA11yElement element)
        {
            return this.A.Matches(element)
                && this.B.Matches(element);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, ConditionDescriptions.And, this.A.ToString(), this.B.ToString());
        }
    } // class
} // namespace
