// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;

using static System.FormattableString;

namespace Axe.Windows.Rules
{
    /// <summary>
    /// Allows the grammatic structure [parent condition] / [child condition] to be expressed.
    /// </summary>
    class TreeDescentCondition : Condition
    {
        private readonly Condition ParentCondition;
        private readonly Condition ChildCondition;

        public TreeDescentCondition(Condition parentCondition, Condition childCondition)
        {
            if (parentCondition == null) throw new ArgumentException();
            if (childCondition == null) throw new ArgumentException();

            this.ParentCondition = parentCondition;
            this.ChildCondition = childCondition;
        }

        public override bool Matches(IA11yElement e)
        {
            if (!this.ParentCondition.Matches(e))
                return false;

            var child = e?.GetFirstChild();

            return this.ChildCondition.Matches(child);
        }

        public override string ToString()
        {
            return Invariant($"{this.ParentCondition} / {this.ChildCondition}");
        }
    } // class
} // namespace
