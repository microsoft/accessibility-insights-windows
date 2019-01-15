// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;

namespace AccessibilityInsights.Rules
{
    class RecursiveCondition : Condition
    {
        private Condition A;

        private void Init(Condition a)
        {
            if (a == null) throw new ArgumentException(nameof(a));

            this.A = a;
        }

        public static RecursiveCondition operator %(RecursiveCondition r, Condition c)
        {
            r.Init(c);
            return r;
        }

        public override bool Matches(IA11yElement e)
        {
            if (A == null) return false;
            if (e == null) throw new ArgumentException();

            return this.A.Matches(e);
        }

        public override string ToString()
        {
            return "Recursive";
        }
    } // class
} // namespace
