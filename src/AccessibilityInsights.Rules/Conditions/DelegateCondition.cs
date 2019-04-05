// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;

namespace Axe.Windows.Rules
{
    class DelegateCondition : Condition
    {
        public delegate bool MatchesDelegate(IA11yElement element);
        private readonly MatchesDelegate _Matches = null;

        public DelegateCondition(MatchesDelegate matches)
        {
            if (matches == null) throw new ArgumentException();

            this._Matches = matches;
        }

        public override bool Matches(IA11yElement element)
        {
            return this._Matches(element);
        }

        public override string ToString()
        {
            return "DelegateCondition";
        }
    } // class
} // namespace
